namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintJobHandlerTests
{
    using Linn.Common.Logging;
    using Linn.PrintService.Messaging.Host.Handlers;
    using Linn.PrintService.Printing.Services;

    using NSubstitute;

    using NUnit.Framework;

    public class ContextBase
    {
        protected IIppPrintingService PrintingService { get; private set; }

        protected ILog Log { get; private set; }

        protected PrintJobMessageHandler Handler { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            this.PrintingService = Substitute.For<IIppPrintingService>();
            this.Log = Substitute.For<ILog>();
            this.Handler = new PrintJobMessageHandler(this.PrintingService, this.Log);
        }
    }
}
