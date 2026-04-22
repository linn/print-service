namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Messaging.RabbitMQ;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingValidInvoiceMessage : ContextBase
    {
        private byte[] pdfData;

        private int documentNumber;

        private string documentType;

        private string printerUri;

        [SetUp]
        public async Task SetUp()
        {
            this.documentNumber = 12345;
            this.documentType = "INV";
            this.printerUri = "ipp://printer.local:631/ipp/print";
            this.pdfData = new byte[] { 1, 2, 3, 4, 5 };

            this.InvoicePrintProxy.GetInvoiceAsPdf(this.documentType, this.documentNumber, false, true)
                .Returns(this.pdfData);

            var message = new Message
                              {
                                  RoutingKey = "print.invoice.document",
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "documentNumber", Encoding.UTF8.GetBytes(this.documentNumber.ToString()) },
                                                    { "documentType", Encoding.UTF8.GetBytes(this.documentType) },
                                                    { "showTermsAndConditions", Encoding.UTF8.GetBytes("false") },
                                                    { "showPrices", Encoding.UTF8.GetBytes("true") },
                                                    { "printerUri", Encoding.UTF8.GetBytes(this.printerUri) }
                                                }
                              };

            await this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public void ShouldCallProxy()
        {
            this.InvoicePrintProxy.Received(1).GetInvoiceAsPdf(
                this.documentType,
                this.documentNumber,
                false,
                true);
        }

        [Test]
        public void ShouldCallPrintService()
        {
            this.PrintingService.Received(1).Print(
                this.printerUri,
                $"Invoice_{this.documentNumber}",
                Arg.Is<byte[]>(b => b.SequenceEqual(this.pdfData)));
        }
    }
}
