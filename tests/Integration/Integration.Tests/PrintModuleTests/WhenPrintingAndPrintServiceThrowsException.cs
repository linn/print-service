namespace Linn.PrintService.Integration.Tests.PrintModuleTests
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using FluentAssertions;

    using Linn.PrintService.Domain.LinnApps.Exceptions;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using NUnit.Framework;

    public class WhenPrintingAndPrintServiceThrowsException : ContextBase
    {
        private string printerUri;
        private string jobName;
        private byte[] data;

        [SetUp]
        public void SetUp()
        {
            this.printerUri = string.Empty;
            this.jobName = "TestJob";
            this.data = Encoding.UTF8.GetBytes("Hello World");

            this.PrintingService.Print(this.printerUri, this.jobName, Arg.Any<byte[]>())
                .Throws(new IppPrintingException("no printer uri"));

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/print-service/print?printerUri={this.printerUri}&jobName={this.jobName}")
            {
                Content = new ByteArrayContent(this.data)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            this.Response = this.Client.SendAsync(request).Result;
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
