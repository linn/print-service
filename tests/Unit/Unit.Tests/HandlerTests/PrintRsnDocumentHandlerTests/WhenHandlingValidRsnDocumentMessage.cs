namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintRsnDocumentHandlerTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Messaging.RabbitMQ;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingValidRsnDocumentMessage : ContextBase
    {
        private byte[] pdfData;

        private int rsnNumber;

        private string copyType;

        private string facilityCode;

        private string printerUri;

        [SetUp]
        public async Task SetUp()
        {
            this.rsnNumber = 12345;
            this.copyType = "original";
            this.facilityCode = "FC001";
            this.printerUri = "ipp://printer.local:631/ipp/print";
            this.pdfData = new byte[] { 1, 2, 3, 4, 5 };

            this.RsnPrintProxy.GetRsnAsPdf(this.rsnNumber, this.copyType, this.facilityCode)
                .Returns(this.pdfData);

            var message = new Message
                              {
                                  RoutingKey = "print.rsn.document",
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "rsnNumber", Encoding.UTF8.GetBytes(this.rsnNumber.ToString()) },
                                                    { "copyType", Encoding.UTF8.GetBytes(this.copyType) },
                                                    { "facilityCode", Encoding.UTF8.GetBytes(this.facilityCode) },
                                                    { "printerUri", Encoding.UTF8.GetBytes(this.printerUri) }
                                                }
                              };

            await this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public void ShouldCallProxy()
        {
            this.RsnPrintProxy.Received(1).GetRsnAsPdf(this.rsnNumber, this.copyType, this.facilityCode);
        }

        [Test]
        public void ShouldCallPrintService()
        {
            this.PrintingService.Received(1).Print(
                this.printerUri,
                $"RSN{this.rsnNumber}",
                Arg.Is<byte[]>(b => b.SequenceEqual(this.pdfData)));
        }
    }
}
