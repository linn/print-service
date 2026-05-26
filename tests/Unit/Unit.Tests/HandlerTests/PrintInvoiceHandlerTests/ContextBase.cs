namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
{
    using Linn.Common.Logging;
    using Linn.Common.Persistence;
    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Domain.LinnApps.Services;
    using Linn.PrintService.Messaging.Handlers;
    using Linn.PrintService.Printing;

    using NSubstitute;

    using NUnit.Framework;

    public class ContextBase
    {
        protected IInvoicePrintProxy InvoicePrintProxy { get; private set; }

        protected IIppPrintingService PrintingService { get; private set; }

        protected ILog Log { get; private set; }

        protected IQueryRepository<PrinterMapping> PrinterMappingRepository { get; private set; }

        protected PrintInvoiceMessageHandler Handler { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            this.InvoicePrintProxy = Substitute.For<IInvoicePrintProxy>();
            this.PrintingService = Substitute.For<IIppPrintingService>();
            this.Log = Substitute.For<ILog>();
            this.PrinterMappingRepository = Substitute.For<IQueryRepository<PrinterMapping>>();

            this.Handler = new PrintInvoiceMessageHandler(
                this.InvoicePrintProxy,
                this.PrintingService,
                this.PrinterMappingRepository,
                this.Log);
        }
    }
}
