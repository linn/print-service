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
        private readonly string baseUri;

        public RsnPrintProxy(IRestClient restClient)
        {
            this.restClient = restClient;
            this.baseUri = ConfigurationManager.Configuration["PROXY_ROOT"]
                           ?? throw new InvalidOperationException("PROXY_ROOT is not configured");
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

            return response.Value;
        }
    }
}
