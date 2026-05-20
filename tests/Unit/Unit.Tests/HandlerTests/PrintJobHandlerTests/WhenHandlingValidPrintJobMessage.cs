namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintJobHandlerTests
{
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Models;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingValidPrintJobMessage : ContextBase
    {
        private string printerUri;

        private string jobName;

        [SetUp]
        public async Task SetUp()
        {
            this.printerUri = "ipp://printer.local:631/ipp/print";
            this.jobName = "TestJob";

            var message = new Message
                              {
                                  RoutingKey = "print.job",
                                  Body = JsonSerializer.SerializeToUtf8Bytes(new PrintJobMessageBody
                                             {
                                                 PrinterUri = this.printerUri,
                                                 JobName = this.jobName,
                                                 Data = new byte[] { 1, 2, 3, 4, 5 }
                                             })
                              };

            await this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public void ShouldCallPrintService()
        {
            this.PrintingService.Received(1).Print(this.printerUri, this.jobName, Arg.Any<byte[]>());
        }
    }
}
