namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintPackingListHandlerTests
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

    public class WhenHandlingValidPackingListMessage : ContextBase
    {
        private byte[] pdfData;

        private int consignmentNumber;

        private string printerGroup;

        private string printerUri;

        [SetUp]
        public async Task SetUp()
        {
            this.consignmentNumber = 67890;
            this.printerGroup = "WAREHOUSE";
            this.printerUri = "ipp://printer.local:631/ipp/print";
            this.pdfData = new byte[] { 1, 2, 3, 4, 5 };

            this.PackingListProxy.GetPackingListAsPdf(this.consignmentNumber)
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
                new PrintPackingListMessageBody
                    {
                        ConsignmentId = this.consignmentNumber,
                        PrinterGroup = this.printerGroup
                    },
                new Dictionary<string, object>(),
                CancellationToken.None);
        }

        [Test]
        public void ShouldCallProxy()
        {
            this.PackingListProxy.Received(1).GetPackingListAsPdf(this.consignmentNumber);
        }

        [Test]
        public void ShouldCallPrintService()
        {
            this.PrintingService.Received(1).Print(
                this.printerUri,
                $"PackingList_{this.consignmentNumber}",
                Arg.Is<byte[]>(b => b.SequenceEqual(this.pdfData)));
        }
    }
}
