namespace Linn.PrintService.Unit.Tests.ProxyTests.RsnPrintProxyTests
{
    using System;
    using System.Net.Http;

    using Linn.PrintService.Proxy;

    using NUnit.Framework;

    public class ContextBase
    {
        protected RsnPrintProxy Sut { get; private set; }

        protected FakeHttpMessageHandler HttpHandler { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            Environment.SetEnvironmentVariable("PROXY_ROOT", "https://app.test.com");

            this.HttpHandler = new FakeHttpMessageHandler();

            var client = new HttpClient(this.HttpHandler);
            this.Sut = new RsnPrintProxy(client);
        }

        [TearDown]
        public void TearDown()
        {
            this.HttpHandler?.Dispose();
        }
    }
}
