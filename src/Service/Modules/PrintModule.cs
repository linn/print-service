namespace Linn.PrintService.Service.Modules
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Linn.Common.Configuration;
    using Linn.Common.Service.Core;

    using Linn.PrintService.Resources.RequestResources;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class PrintModule : IModule
    {
        // Hardcoded credentials for the print service.
        private static readonly string Username = ConfigurationManager.Configuration["PRINT_USERNAME"]; 
        private static readonly string Password = ConfigurationManager.Configuration["PRINT_PASSWORD"];

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // POST endpoint to send a print job.
            app.MapPost("/print", this.Print);
        }

        /// <summary>
        /// Builds the IPP 1.0 payload with header, attributes, and document bytes.
        /// </summary>
        private static byte[] BuildPayload(string printerUri, string user, string jobName, byte[] documentBytes)
        {
            using (var ms = new MemoryStream())
            {
                // IPP header: version 1.0 (0x01, 0x00)
                ms.WriteByte(0x01);
                ms.WriteByte(0x00);

                // Operation ID: Print-Job (0x00, 0x02)
                ms.WriteByte(0x00);
                ms.WriteByte(0x02);

                // Request ID: 1 (big endian)
                ms.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1)), 0, 4);

                // Start of operation attributes
                ms.WriteByte(0x01); // operation-attributes-tag

                byte[] attrs = Array.Empty<byte>();
                attrs = AddAttr(attrs, 0x47, "attributes-charset", "utf-8");
                attrs = AddAttr(attrs, 0x48, "attributes-natural-language", "en");
                attrs = AddAttr(attrs, 0x45, "printer-uri", printerUri);

                // Include username for tracking/logging. Optional part.
                attrs = AddAttr(attrs, 0x42, "requesting-user-name", user);
                attrs = AddAttr(attrs, 0x42, "job-name", jobName);

                // Application/pdf does not work. This is far more flexible anyway.
                attrs = AddAttr(attrs, 0x49, "document-format", "application/octet-stream");

                ms.Write(attrs, 0, attrs.Length);

                // End of attributes tag
                ms.WriteByte(0x03);

                // Append PDF file bytes
                ms.Write(documentBytes, 0, documentBytes.Length);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Adds a single attribute to the IPP attribute block.
        /// </summary>
        private static byte[] AddAttr(byte[] attrs, byte tag, string name, string value)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(attrs, 0, attrs.Length);
                ms.WriteByte(tag);

                byte[] nameBytes = Encoding.UTF8.GetBytes(name);
                byte[] valueBytes = Encoding.UTF8.GetBytes(value);

                // Write name length
                ms.Write(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)nameBytes.Length)), 0, 2);
                ms.Write(nameBytes, 0, nameBytes.Length);

                // Write value length
                ms.Write(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)valueBytes.Length)), 0, 2);
                ms.Write(valueBytes, 0, valueBytes.Length);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Strictly for debug purposes. Generates a hex preview of the given byte array, limited to a maximum length.
        /// </summary>
        private static string HexPreview(byte[] data, int maxLength = 128)
        {
            var sb = new StringBuilder();
            int length = Math.Min(data.Length, maxLength);
            for (int i = 0; i < length; i++)
            {
                sb.AppendFormat("{0:x2}", data[i]);
            }

            if (data.Length > maxLength)
            {
                sb.Append("...");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sends the IPP print job via HTTP POST with hardcoded explicit credentials.
        /// </summary>
        private static async Task<(bool success, int statusCode, string preview)> SendPrintJob(
            string uri, byte[] ippPayload, string username, string password)
        {
            try
            {
                var credentials = new NetworkCredential(username, password);

                using (var handler = new HttpClientHandler
                {
                    Credentials = credentials,   // explicit hardcoded credentials
                    PreAuthenticate = true       // attempt to send creds with initial request
                })

                // Create HTTP client to send the request
                using (var client = new HttpClient(handler))
                {
                    // Create HTTP content with the IPP payload
                    var content = new ByteArrayContent(ippPayload);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/ipp");

                    // POST the request to the printer URI
                    var response = await client.PostAsync(uri, content);

                    // Read the response bytes
                    var respBytes = await response.Content.ReadAsByteArrayAsync();

                    return (response.IsSuccessStatusCode,
                            (int)response.StatusCode,
                            HexPreview(respBytes, 256));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                return (false, 500, e.Message);
            }
        }

        private async Task Print(HttpRequest req, HttpResponse res, PrintJobRequestResource resource)
        {
            Console.WriteLine($"Printer URI: {resource.PrinterUri}");
            Console.WriteLine($"User: {Username}");
            Console.WriteLine($"Job Name: {resource.JobName}");

            if (resource.Data.Length == 0)
            {
                res.StatusCode = 400;
                await res.WriteAsync("Error: Empty data.");
                return;
            }

            byte[] documentBytes = resource.Data;

            // Build IPP payload
            byte[] ippPayload = BuildPayload(resource.PrinterUri, Username, resource.JobName ?? "PrintJob", documentBytes);

            // Debug purposes
            Console.WriteLine("IPP 1.0 Print-Job (hex preview):");
            Console.WriteLine(HexPreview(ippPayload));

            // Send print job (with hardcoded credentials)
            var response = await SendPrintJob(resource.PrinterUri, ippPayload, Username, Password);

            res.ContentType = "application/json";
            await res.WriteAsJsonAsync(new
            {
                success = response.success,
                printer = resource.PrinterUri,
                job = resource.JobName,
                httpStatus = response.statusCode,
                responsePreview = response.preview
            });
        }
    }
}
