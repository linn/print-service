namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintRsnDocumentHandlerTests
{
    using Linn.Common.Logging;
    using Linn.PrintService.Messaging.Handlers;
    using Linn.PrintService.Printing.Services;

    using NSubstitute;

    using NUnit.Framework;

    public class ContextBase
    {
        protected IRsnPrintProxy RsnPrintProxy { get; private set; }

        protected IIppPrintingService PrintingService { get; private set; }

        protected ILog Log { get; private set; }

        protected PrintRsnDocumentMessageHandler Handler { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            this.RsnPrintProxy = Substitute.For<IRsnPrintProxy>();
            this.PrintingService = Substitute.For<IIppPrintingService>();
            this.Log = Substitute.For<ILog>();
            this.Handler = new PrintRsnDocumentMessageHandler(
                this.RsnPrintProxy,
                this.PrintingService,
                this.Log);
        }
    }
}
