namespace Linn.PrintService.Integration.Tests.HandlerTests.PrintJobHandlerTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

            var message = new Message
                              {
                                  RoutingKey = "print.job",
                                  Body = this.pdfData,
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "printerUri", Encoding.UTF8.GetBytes(this.printerUri) },
                                                    { "jobName", Encoding.UTF8.GetBytes(this.jobName) }
                                                }
                              };

            await this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public void ShouldCallPrintService()
        {
            this.PrintingService.Received(1).Print(
                this.printerUri,
                this.jobName,
                Arg.Is<byte[]>(b => b.SequenceEqual(this.pdfData)));
        }
    }
}
