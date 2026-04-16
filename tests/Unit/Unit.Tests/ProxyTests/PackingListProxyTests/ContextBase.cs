namespace Linn.PrintService.Unit.Tests.ProxyTests.PackingListProxyTests
{
    using System;
    using System.Net.Http;

    using Linn.PrintService.Proxy;

    using NUnit.Framework;

    public class ContextBase
    {
        protected PackingListProxy Sut { get; private set; }

        protected FakeHttpMessageHandler HttpHandler { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            Environment.SetEnvironmentVariable("PROXY_ROOT", "https://app.test.com");

            this.HttpHandler = new FakeHttpMessageHandler();

            var client = new HttpClient(this.HttpHandler);
            this.Sut = new PackingListProxy(client);
        }

        [TearDown]
        public void TearDown()
        {
            this.HttpHandler?.Dispose();
        }
    }
}
