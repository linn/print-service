namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Messaging.Models;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingValidInvoiceMessage : ContextBase
    {
        private byte[] pdfData;

        private int documentNumber;

        private string documentType;

        private string printerGroup;

        private string printerUri;

        [SetUp]
        public async Task SetUp()
        {
            this.documentNumber = 12345;
            this.documentType = "INV";
            this.printerGroup = "ACCOUNTS";
            this.printerUri = "ipp://printer.local:631/ipp/print";
            this.pdfData = new byte[] { 1, 2, 3, 4, 5 };

            this.InvoicePrintProxy.GetInvoiceAsPdf(this.documentType, this.documentNumber, false, true)
                .Returns(this.pdfData);

            this.PrinterMappingRepository
                .FindByAsync(Arg.Any<Expression<Func<PrinterMapping, bool>>>())
                .Returns(new PrinterMapping
                    {
                        PrinterGroup = this.printerGroup,
                        PrinterUri = this.printerUri,
                        PrinterType = "A4",
                        DefaultForGroup = "Y"
                    });

            await this.Handler.HandleAsync(
                new PrintInvoiceMessageBody
                    {
                        DocumentNumber = this.documentNumber,
                        DocumentType = this.documentType,
                        ShowTermsAndConditions = false,
                        ShowPrices = true,
                        PrinterGroup = this.printerGroup
                    },
                new Dictionary<string, object>(),
                CancellationToken.None);
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
