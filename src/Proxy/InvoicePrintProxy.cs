namespace Linn.PrintService.Proxy
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Linn.Common.Configuration;
    using Linn.PrintService.Printing.Services;

    public class InvoicePrintProxy : IInvoicePrintProxy
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;

        public InvoicePrintProxy(HttpClient httpClient)
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

        public async Task<byte[]> GetInvoiceAsPdf(
            string documentType,
            int documentNumber,
            bool showTerms,
            bool showPrices)
        {
            var encodedDocumentType = Uri.EscapeDataString(documentType ?? string.Empty);
            var uri = new Uri(
                this.baseUri,
                $"/sales/documents/pdf/{encodedDocumentType}/{documentNumber}?showTerms={showTerms}&showPrices={showPrices}");

            using (var response = await this.httpClient.GetAsync(uri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Invoice PDF request failed with status {(int)response.StatusCode} ({response.ReasonPhrase}): {errorBody}");
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
        }
    }
}
