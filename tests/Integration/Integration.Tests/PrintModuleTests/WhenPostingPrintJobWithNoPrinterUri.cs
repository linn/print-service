namespace Linn.PrintService.Integration.Tests.PrintModuleTests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using FluentAssertions;

    using Linn.PrintService.Printing;
    using Linn.PrintService.Printing.Exceptions;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using NUnit.Framework;

    public class WhenPostingPrintJobWithNoPrinterUri : ContextBase
    {
        private HttpContent requestContent;
        private string printerUri;
        private string jobName;
        private byte[] data;

        [SetUp]
        public void SetUp()
        {
            this.printerUri = string.Empty;
            this.jobName = "TestJob";
            this.data = Encoding.UTF8.GetBytes("Hello World");

            this.requestContent = new ByteArrayContent(this.data);
            this.requestContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
           
            this.PrintingService.Print(this.printerUri, this.jobName, this.data).Throws(new IppPrintingException("no printer uri"));

            this.Response = this.Client.PostAsync(
                $"/print-service/print?printerUri={this.printerUri}&jobName={this.jobName}",
                this.requestContent).Result;
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
