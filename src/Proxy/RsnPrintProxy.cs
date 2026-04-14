namespace Linn.PrintService.Proxy
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Linn.Common.Configuration;
    using Linn.PrintService.Printing.Services;

    public class RsnPrintProxy : IRsnPrintProxy
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string baseUri;

        public RsnPrintProxy(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.baseUri = ConfigurationManager.Configuration["PROXY_ROOT"]
                           ?? throw new InvalidOperationException("PROXY_ROOT is not configured");
        }

        public async Task<byte[]> GetRsnPrintAsPdf(int rsnNumber, string copyType, string facilityCode)
        {
            var encodedCopyType = Uri.EscapeDataString(copyType ?? string.Empty);
            var encodedFacilityCode = Uri.EscapeDataString(facilityCode ?? string.Empty);
            var uri = $"{this.baseUri}/service/rsns/print/pdf?rsnNumber={rsnNumber}&copyType={encodedCopyType}&facilityCode={encodedFacilityCode}";

            var client = this.httpClientFactory.CreateClient("RsnPdfProxy");
            using (var response = await client.GetAsync(new Uri(uri, UriKind.RelativeOrAbsolute)))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"RSN PDF request failed with status {(int)response.StatusCode} ({response.ReasonPhrase}): {errorBody}");
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
        }
    }
}
