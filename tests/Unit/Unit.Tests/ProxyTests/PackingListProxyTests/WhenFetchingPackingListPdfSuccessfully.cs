namespace Linn.PrintService.Unit.Tests.ProxyTests.PackingListProxyTests
{
    using System.Net;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NUnit.Framework;

    public class WhenFetchingPackingListPdfSuccessfully : ContextBase
    {
        private byte[] pdfData;

        private byte[] result;

        [SetUp]
        public async Task SetUp()
        {
            this.pdfData = new byte[] { 0x25, 0x50, 0x44, 0x46 };
            this.HttpHandler.StatusCode = HttpStatusCode.OK;
            this.HttpHandler.ResponseBytes = this.pdfData;

            this.result = await this.Sut.GetPackingListAsPdf(67890);
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
                .Should().Be("https://app.test.com/stores2/consignments/67890/packing-list/pdf");
        }

        [Test]
        public void ShouldUseGetMethod()
        {
            this.HttpHandler.LastRequest.Method.Should().Be(System.Net.Http.HttpMethod.Get);
        }
    }
}
