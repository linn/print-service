namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintPackingListHandlerTests
{
    using Linn.Common.Logging;
    using Linn.PrintService.Messaging.Host.Handlers;
    using Linn.PrintService.Printing.Services;

    using NSubstitute;

    using NUnit.Framework;

    public class ContextBase
    {
        protected IPackingListProxy PackingListProxy { get; private set; }

        protected IIppPrintingService PrintingService { get; private set; }

        protected ILog Log { get; private set; }

        protected PrintPackingListMessageHandler Handler { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            this.PackingListProxy = Substitute.For<IPackingListProxy>();
            this.PrintingService = Substitute.For<IIppPrintingService>();
            this.Log = Substitute.For<ILog>();
            this.Handler = new PrintPackingListMessageHandler(
                this.PackingListProxy,
                this.PrintingService,
                this.Log);
        }
    }
}
