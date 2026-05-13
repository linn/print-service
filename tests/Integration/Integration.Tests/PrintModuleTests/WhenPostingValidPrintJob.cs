namespace Linn.PrintService.Integration.Tests.PrintModuleTests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Integration.Tests.Extensions;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenPostingValidPrintJob : ContextBase
    {
        private HttpContent requestContent;
        private string printerUri;
        private string jobName;
        private byte[] data;

        [SetUp]
        public void SetUp()
        {
            this.printerUri = "http://printer.local:631/ipp/print";
            this.jobName = "TestJob";
            this.data = Encoding.UTF8.GetBytes("Hello World");

            this.PrintingService.Print(this.printerUri, this.jobName, Arg.Any<byte[]>())
                .Returns(Task.FromResult(new PrintResult { Success = true, HttpStatus = 200 }));

            this.requestContent = new ByteArrayContent(this.data);
            this.requestContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            this.Response = this.Client.Post(
                $"/print-service/print?printerUri={this.printerUri}&jobName={this.jobName}",
                this.requestContent,
                with => { with.Accept("application/json"); }).Result;
        }

        [Test]
        public void ShouldReturnOk()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnJsonContentType()
        {
            this.Response.Content.Headers.ContentType.Should().NotBeNull();
            this.Response.Content.Headers.ContentType?.ToString().Should().Contain("application/json");
        }

        [Test]
        public void ShouldCallPrintingService()
        {
            // Byte instances are not equal reference wise
            this.PrintingService.Received(1).Print(this.printerUri, this.jobName, Arg.Is<byte[]>(b => b.SequenceEqual(this.data)));
        }
    }
}
