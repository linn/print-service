namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintPackingListHandlerTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.PrintService.Messaging.Models;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingValidPackingListMessage : ContextBase
    {
        private byte[] pdfData;

        private int consignmentNumber;

        private string printerUri;

        [SetUp]
        public async Task SetUp()
        {
            this.consignmentNumber = 67890;
            this.printerUri = "ipp://printer.local:631/ipp/print";
            this.pdfData = new byte[] { 1, 2, 3, 4, 5 };

            this.PackingListProxy.GetPackingListAsPdf(this.consignmentNumber)
                .Returns(this.pdfData);

            await this.Handler.HandleAsync(
                new PrintPackingListMessageBody
                    {
                        ConsignmentId = this.consignmentNumber,
                        PrinterUri = this.printerUri
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
