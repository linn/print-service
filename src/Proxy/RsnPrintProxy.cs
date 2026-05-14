namespace Linn.PrintService.Proxy
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Linn.Common.Configuration;
    using Linn.PrintService.Printing.Services;

    public class RsnPrintProxy : IRsnPrintProxy
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;

        public RsnPrintProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            var proxyRoot = ConfigurationManager.Configuration["PROXY_ROOT"]
                            ?? throw new InvalidOperationException("PROXY_ROOT is not configured");

            if (!Uri.TryCreate(proxyRoot, UriKind.Absolute, out var parsedUri))
            {
                throw new InvalidOperationException(
                    $"PROXY_ROOT must be a valid absolute URI, got: '{proxyRoot}'");
            }

            this.baseUri = parsedUri;
        }

        public async Task<byte[]> GetRsnAsPdf(int rsnNumber, string copyType, string facilityCode)
        {
            var encodedCopyType = Uri.EscapeDataString(copyType ?? string.Empty);
            var encodedFacilityCode = Uri.EscapeDataString(facilityCode ?? string.Empty);
            var uri = new Uri(
                this.baseUri,
                $"/service/rsns/print/pdf?rsnNumber={rsnNumber}&copyType={encodedCopyType}&facilityCode={encodedFacilityCode}");

            using (var response = await this.httpClient.GetAsync(uri))
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
