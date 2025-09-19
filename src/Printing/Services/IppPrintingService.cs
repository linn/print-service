namespace Linn.PrintService.Printing.Services
{
    using System.Net;
    using System.Text;

    using Linn.PrintService.Printing.Exceptions;

    public class IppPrintingService : IIppPrintingService
    {
        private readonly string username;
        private readonly string password;

        public IppPrintingService(string userName, string password)
        {
            this.username = userName;
            this.password = password;
        }

        public async Task<PrintResult> Print(string printerUri, string jobName, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(printerUri))
            {
                throw new IppPrintingException(message: "printerUri is required");
            }

            if (data.Length == 0)
            {
                throw new IppPrintingException(message: "Data cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(jobName))
            {
                jobName = "PrintJob";
            }

            var ippPayload = this.BuildPayload(printerUri, this.username, jobName, data);
            return await this.SendIppRequest(printerUri, ippPayload);
        }

        public async Task<PrintResult> GetStatus(string printerUri)
        {
            if (string.IsNullOrWhiteSpace(printerUri))
            {
                throw new IppPrintingException("printerUri is required");
            }

            var ippPayload = this.BuildStatusPayload(printerUri, this.username);

            var result = await this.SendIppRequest(printerUri, ippPayload);

            // Mark "State" field — you can parse proper IPP attributes later if needed.
            result.State = result.Success ? "unknown" : "error";
            return result;
        }

        private byte[] BuildPayload(string printerUri, string user, string jobName, byte[] documentBytes)
        {
            using (var ms = new MemoryStream())
            {
                // IPP header
                ms.WriteByte(0x01); // major version
                ms.WriteByte(0x00); // minor version
                ms.WriteByte(0x00); // operation-id high
                ms.WriteByte(0x02); // operation-id low (Print-Job)
                ms.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1)), 0, 4); // request-id

                // operation-attributes-tag
                ms.WriteByte(0x01);

                var attrs = Array.Empty<byte>();
                attrs = this.AddAttr(attrs, 0x47, "attributes-charset", "utf-8");
                attrs = this.AddAttr(attrs, 0x48, "attributes-natural-language", "en");
                attrs = this.AddAttr(attrs, 0x45, "printer-uri", printerUri);
                attrs = this.AddAttr(attrs, 0x42, "requesting-user-name", user);
                attrs = this.AddAttr(attrs, 0x42, "job-name", jobName);
                attrs = this.AddAttr(attrs, 0x49, "document-format", "application/octet-stream");

                ms.Write(attrs, 0, attrs.Length);

                // end-of-attributes-tag
                ms.WriteByte(0x03);

                // document bytes
                ms.Write(documentBytes, 0, documentBytes.Length);

                return ms.ToArray();
            }
        }

        private byte[] BuildStatusPayload(string printerUri, string user)
        {
            using (var ms = new MemoryStream())
            {
                ms.WriteByte(0x01); // major version
                ms.WriteByte(0x00); // minor version
                ms.WriteByte(0x00); // op-id high
                ms.WriteByte(0x0B); // Get-Printer-Attributes
                ms.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1)), 0, 4);

                ms.WriteByte(0x01); // operation-attributes-tag

                var attrs = Array.Empty<byte>();
                attrs = this.AddAttr(attrs, 0x47, "attributes-charset", "utf-8");
                attrs = this.AddAttr(attrs, 0x48, "attributes-natural-language", "en");
                attrs = this.AddAttr(attrs, 0x45, "printer-uri", printerUri);
                attrs = this.AddAttr(attrs, 0x42, "requesting-user-name", user);

                ms.Write(attrs, 0, attrs.Length);

                ms.WriteByte(0x03); // end-of-attributes-tag

                return ms.ToArray();
            }
        }

        private byte[] AddAttr(byte[] attrs, byte tag, string name, string value)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(attrs, 0, attrs.Length);
                ms.WriteByte(tag);

                var nameBytes = Encoding.UTF8.GetBytes(name);
                var valueBytes = Encoding.UTF8.GetBytes(value);

                ms.Write(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)nameBytes.Length)), 0, 2);
                ms.Write(nameBytes, 0, nameBytes.Length);

                ms.Write(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)valueBytes.Length)), 0, 2);
                ms.Write(valueBytes, 0, valueBytes.Length);

                return ms.ToArray();
            }
        }

        private async Task<PrintResult> SendIppRequest(string uri, byte[] ippPayload)
        {
            try
            {
                var credentials = new NetworkCredential(this.username, this.password);

                using (var handler = new HttpClientHandler
                {
                    Credentials = credentials,
                    PreAuthenticate = true
                })
                using (var client = new HttpClient(handler))
                using (var content = new ByteArrayContent(ippPayload))
                {
                    content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/ipp");

                    var response = await client.PostAsync(uri, content);
                    var respBytes = await response.Content.ReadAsByteArrayAsync();

                    return new PrintResult
                    {
                        Success = response.IsSuccessStatusCode,
                        HttpStatus = (int)response.StatusCode,
                        ResponsePreview = this.HexPreview(respBytes, 256)
                    };
                }
            }
            catch (Exception ex)
            {
                return new PrintResult
                {
                    Success = false,
                    HttpStatus = 500,
                    ResponsePreview = ex.Message
                };
            }
        }

        private string HexPreview(byte[] data, int maxLength)
        {
            var sb = new StringBuilder();
            var length = Math.Min(data.Length, maxLength);

            for (var i = 0; i < length; i++)
            {
                sb.AppendFormat("{0:x2}", data[i]);
            }

            if (data.Length > maxLength)
            {
                sb.Append("...");
            }

            return sb.ToString();
        }
    }
}
