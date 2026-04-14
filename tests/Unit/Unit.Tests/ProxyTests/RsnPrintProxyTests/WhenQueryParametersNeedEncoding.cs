namespace Linn.PrintService.Unit.Tests.ProxyTests.RsnPrintProxyTests
{
    using System.Net;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NUnit.Framework;

    public class WhenQueryParametersNeedEncoding : ContextBase
    {
        [SetUp]
        public async Task SetUp()
        {
            this.HttpHandler.StatusCode = HttpStatusCode.OK;
            this.HttpHandler.ResponseBytes = new byte[] { 1, 2, 3 };

            await this.Sut.GetRsnPrintAsPdf(100, "Copy & Type", "FC/001");
        }

        [Test]
        public void ShouldEncodeSpecialCharacters()
        {
            var requestUri = this.HttpHandler.LastRequest.RequestUri.AbsoluteUri;
            requestUri.Should().Contain("copyType=Copy%20%26%20Type");
            requestUri.Should().Contain("facilityCode=FC%2F001");
        }
    }
}
