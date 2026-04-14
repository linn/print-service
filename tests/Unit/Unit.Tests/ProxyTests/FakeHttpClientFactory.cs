namespace Linn.PrintService.Unit.Tests.ProxyTests
{
    using System.Net.Http;

    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler handler;

        public FakeHttpClientFactory(HttpMessageHandler handler)
        {
            this.handler = handler;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(this.handler);
        }
    }
}
