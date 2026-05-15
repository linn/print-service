namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintJobHandlerTests
{
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Messaging.RabbitMQ;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingValidPrintJobMessage : ContextBase
    {
        private byte[] pdfData;

        private string printerUri;

        private string jobName;

        [SetUp]
        public async Task SetUp()
        {
            this.pdfData = new byte[] { 1, 2, 3, 4, 5 };
            this.printerUri = "ipp://printer.local:631/ipp/print";
            this.jobName = "TestJob";

            var bodyJson = JsonSerializer.Serialize(new
            {
                printerUri = this.printerUri,
                jobName = this.jobName
            });

            var message = new Message
                              {
                                  RoutingKey = "print.job",
                                  Body = Encoding.UTF8.GetBytes(bodyJson)
                              };

            await this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public void ShouldCallPrintService()
        {
            this.PrintingService.Received(1).Print(
                this.printerUri,
                this.jobName,
                Arg.Any<byte[]>());
        }
    }
}
