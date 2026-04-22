namespace Linn.PrintService.Unit.Tests.ProxyTests.InvoicePrintProxyTests
{
    using System.Net;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NUnit.Framework;

    public class WhenFetchingInvoicePdfSuccessfully : ContextBase
    {
        private byte[] pdfData;

        private byte[] result;

        [SetUp]
        public async Task SetUp()
        {
            this.pdfData = new byte[] { 0x25, 0x50, 0x44, 0x46 };
            this.HttpHandler.StatusCode = HttpStatusCode.OK;
            this.HttpHandler.ResponseBytes = this.pdfData;

            this.result = await this.Sut.GetInvoiceAsPdf("INV", 12345, false, true);
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
                .Should().Be("https://app.test.com/sales/documents/pdf/INV/12345?showTerms=False&showPrices=True");
        }
    }
}
