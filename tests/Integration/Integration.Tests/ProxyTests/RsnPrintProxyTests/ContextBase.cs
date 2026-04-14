namespace Linn.PrintService.Integration.Tests.ProxyTests.RsnPrintProxyTests
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

            var factory = new FakeHttpClientFactory(this.HttpHandler);
            this.Sut = new RsnPrintProxy(factory);
        }

        [TearDown]
        public void TearDown()
        {
            this.HttpHandler?.Dispose();
        }
    }
}
