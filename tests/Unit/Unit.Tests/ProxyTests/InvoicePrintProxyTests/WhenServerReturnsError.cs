namespace Linn.PrintService.Unit.Tests.ProxyTests.InvoicePrintProxyTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NUnit.Framework;

    public class WhenServerReturnsError : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            this.HttpHandler.StatusCode = HttpStatusCode.InternalServerError;
            this.HttpHandler.ResponseString = "Document not found";

            this.action = () => this.Sut.GetInvoiceAsPdf("INV", 99999, false, true);
        }

        [Test]
        public async Task ShouldThrowHttpRequestException()
        {
            await this.action.Should().ThrowAsync<HttpRequestException>()
                .WithMessage("*500*");
        }

        [Test]
        public async Task ShouldIncludeResponseBodyInException()
        {
            await this.action.Should().ThrowAsync<HttpRequestException>()
                .WithMessage("*Document not found*");
        }
    }
}
