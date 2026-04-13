namespace Linn.PrintService.Proxy
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Configuration;
    using Linn.Common.Proxy;
    using Linn.PrintService.Printing.Services;

    public class RsnPrintProxy : IRsnPrintProxy
    {
        private readonly IRestClient restClient;
        private readonly string baseUri = ConfigurationManager.Configuration["PROXY_ROOT"]!;

        public RsnPrintProxy(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<byte[]> GetRsnPrintAsPdf(int rsnNumber, string copyType, string facilityCode)
        {
            var uri = $"{this.baseUri}/service/rsns/print/pdf?rsnNumber={rsnNumber}&copyType={copyType}&facilityCode={facilityCode}";
            var cancellationToken = CancellationToken.None;

            var response = await this.restClient.Get<byte[]>(
                               cancellationToken,
                               new Uri(uri, UriKind.RelativeOrAbsolute),
                               new Dictionary<string, string>(),
                               new Dictionary<string, string[]>());

            return response.Value
                   ?? throw new InvalidOperationException(
                       $"Empty response when fetching RSN {rsnNumber} PDF");
        }
    }
}
