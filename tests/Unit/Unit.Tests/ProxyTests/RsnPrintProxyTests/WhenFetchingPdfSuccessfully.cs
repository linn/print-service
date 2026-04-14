namespace Linn.PrintService.Unit.Tests.ProxyTests.RsnPrintProxyTests
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NUnit.Framework;

    public class WhenFetchingPdfSuccessfully : ContextBase
    {
        private byte[] pdfData;

        private byte[] result;

        [SetUp]
        public async Task SetUp()
        {
            this.pdfData = new byte[] { 0x25, 0x50, 0x44, 0x46 };
            this.HttpHandler.StatusCode = HttpStatusCode.OK;
            this.HttpHandler.ResponseBytes = this.pdfData;

            this.result = await this.Sut.GetRsnAsPdf(12345, "Service", "FC001");
        }

        [Test]
        public void ShouldReturnPdfBytes()
        {
            this.result.Should().Equal(this.pdfData);
        }

        [Test]
        public void ShouldBuildCorrectUri()
        {
            this.HttpHandler.LastRequest.RequestUri.AbsoluteUri
                .Should().Be("https://app.test.com/service/rsns/print/pdf?rsnNumber=12345&copyType=Service&facilityCode=FC001");
        }

        [Test]
        public void ShouldUseGetMethod()
        {
            this.HttpHandler.LastRequest.Method.Should().Be(System.Net.Http.HttpMethod.Get);
        }
    }
}
