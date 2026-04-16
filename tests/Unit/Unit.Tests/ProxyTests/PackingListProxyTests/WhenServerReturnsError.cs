namespace Linn.PrintService.Unit.Tests.ProxyTests.PackingListProxyTests
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
            this.HttpHandler.ResponseString = "Consignment not found";

            this.action = () => this.Sut.GetPackingListAsPdf(99999);
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
                .WithMessage("*Consignment not found*");
        }
    }
}
