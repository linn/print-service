namespace Linn.PrintService.Unit.Tests.ProxyTests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public byte[] ResponseBytes { get; set; } = new byte[0];

        public string ResponseString { get; set; }

        public HttpRequestMessage LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            this.LastRequest = request;

            var content = this.ResponseString != null
                              ? (HttpContent)new StringContent(this.ResponseString)
                              : new ByteArrayContent(this.ResponseBytes);

            var response = new HttpResponseMessage(this.StatusCode)
                               {
                                   Content = content,
                                   RequestMessage = request
                               };

            return Task.FromResult(response);
        }
    }
}
