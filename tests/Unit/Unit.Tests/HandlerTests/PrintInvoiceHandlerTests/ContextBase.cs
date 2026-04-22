namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
{
    using Linn.Common.Logging;
    using Linn.PrintService.Messaging.Handlers;
    using Linn.PrintService.Printing.Services;

    using NSubstitute;

    using NUnit.Framework;

    public class ContextBase
    {
        protected IInvoicePrintProxy InvoicePrintProxy { get; private set; }

        protected IIppPrintingService PrintingService { get; private set; }

        protected ILog Log { get; private set; }

        protected PrintInvoiceMessageHandler Handler { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            this.InvoicePrintProxy = Substitute.For<IInvoicePrintProxy>();
            this.PrintingService = Substitute.For<IIppPrintingService>();
            this.Log = Substitute.For<ILog>();
            this.Handler = new PrintInvoiceMessageHandler(
                this.InvoicePrintProxy,
                this.PrintingService,
                this.Log);
        }
    }
}
