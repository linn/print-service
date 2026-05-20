namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
{
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Models;

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
                                  Body = JsonSerializer.SerializeToUtf8Bytes(new PrintInvoiceMessageBody
                                             {
                                                 DocumentNumber = this.documentNumber,
                                                 DocumentType = this.documentType,
                                                 ShowTermsAndConditions = false,
                                                 ShowPrices = true,
                                                 PrinterUri = this.printerUri
                                             })
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
