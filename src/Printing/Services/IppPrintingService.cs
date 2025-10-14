namespace Linn.PrintService.Printing.Services
{
    using System.Net;
    using System.Text;
    using System.Text.Json;

    using Linn.Common.Logging;
    using Linn.PrintService.Printing.Exceptions;

    public class IppPrintingService : IIppPrintingService
    {
        private readonly ILog log;
        private readonly HttpClient httpClient;

        public IppPrintingService(ILog log, HttpClient httpClient)
        {
            this.log = log;
            this.httpClient = httpClient;
        }

        public async Task<PrintResult> Print(string printerUri, string jobName, byte[] data)
        {
            this.log.Info($"Print requested: printerUri={printerUri}, jobName={jobName}, dataLength={data?.Length ?? 0}");

            if (string.IsNullOrWhiteSpace(printerUri))
            {
                throw new IppPrintingException("printerUri is required");
            }

            if (data.Length == 0)
            {
                throw new IppPrintingException(message: "Data cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(jobName))
            {
                jobName = "PrintJob";
            }

            try
            {
                byte[] documentBytes;

                var decoded = JsonSerializer.Deserialize<byte[]>(data);
                if (decoded != null)
                {
                    this.log.Info("Document bytes successfully decoded.");
                    documentBytes = decoded;
                }
                else
                {
                    this.log.Error("Failed to decode document bytes.");
                    throw new IppPrintingException("Failed to decode document bytes.");
                }

                var ippPayload = this.BuildPayload(printerUri, jobName, documentBytes);
                var result = await this.SendIppRequest(printerUri, ippPayload);

                this.log.Info($"Print completed: printerUri={printerUri}, "
                              + $"jobName={jobName}, "
                              + $"success={result.Success}, "
                              + $"httpStatus={result.HttpStatus}, "
                              + $"message={result.ResponsePreview}"); 
                return result;
            }
            catch (Exception ex)
            {
                this.log.Error($"Print exception: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<PrintResult> GetDetailedStatus(string printerUri)
        {
            this.log.Info($"Status requested: printerUri={printerUri}");

            if (string.IsNullOrWhiteSpace(printerUri))
            {
                throw new IppPrintingException("printerUri is required");
            }

            try
            {
                var ippPayload = this.BuildStatusPayload(printerUri);
                var result = await this.SendIppRequest(printerUri, ippPayload);

                result.State = result.Success ? "unknown" : "error";
                this.log.Info($"GetStatus completed: printerUri={printerUri}, " +
                              $"success={result.Success}, httpStatus={result.HttpStatus}, " +
                              $"message={result.ResponsePreview}");

                return result;
            }
            catch (Exception ex)
            {
                this.log.Error($"GetStatus exception: {ex.Message}", ex);
                throw new IppPrintingException("Error occurred while retrieving printer status.", ex);
            }
        }

        private byte[] BuildPayload(string printerUri, string jobName, byte[] documentBytes)
        {
            this.log.Info($"Building IPP payload for printerUri={printerUri}, jobName={jobName}, documentBytesLength={documentBytes?.Length ?? 0}");
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
                attrs = this.AddAttr(attrs, 0x42, "job-name", jobName);
                attrs = this.AddAttr(attrs, 0x49, "document-format", "application/octet-stream");

                ms.Write(attrs, 0, attrs.Length);

                // end-of-attributes-tag
                ms.WriteByte(0x03);
                
                // document bytes
                ms.Write(documentBytes, 0, documentBytes.Length);
                this.log.Info("IPP payload built successfully.");

                return ms.ToArray();
            }
        }

        private byte[] BuildStatusPayload(string printerUri)
        {
            this.log.Info($"Building status payload for printerUri={printerUri}");
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

                ms.Write(attrs, 0, attrs.Length);

                ms.WriteByte(0x03); // end-of-attributes-tag
                
                this.log.Info("Status payload built successfully.");
                return ms.ToArray();
            }
        }

        private byte[] AddAttr(byte[] attrs, byte tag, string name, string value)
        {
            this.log.Info($"Adding attribute: tag=0x{tag:x2}, name={name}, value={value}");
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

                this.log.Info($"Attribute {name} added.");
                return ms.ToArray();
            }
        }

        private async Task<PrintResult> SendIppRequest(string uri, byte[] ippPayload)
        {
            this.log.Info($"Sending IPP request to {uri} with payload length {ippPayload?.Length ?? 0}");
            try
            {
                using (var content = new ByteArrayContent(ippPayload))
                {
                    content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/ipp");

                    var response = await this.httpClient.PostAsync(uri, content);
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
                this.log.Error($"IPP request failed: {ex.Message}", ex);
                throw new IppPrintingException("Error occurred while sending IPP request.", ex);
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
