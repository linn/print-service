namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintJobHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Logging;
    using Linn.PrintService.Messaging.Models;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingPrintJobMessageWithMissingHeaders : ContextBase
    {
        [SetUp]
        public async Task SetUp()
        {
            await this.Handler.HandleAsync(
                new PrintJobMessageBody(),
                new Dictionary<string, object>(),
                CancellationToken.None);
        }

        [Test]
        public void ShouldNotCallPrintService()
        {
            this.PrintingService.DidNotReceive().Print(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<byte[]>());
        }

        [Test]
        public void ShouldLogError()
        {
            this.Log.Received(1).Write(
                LoggingLevel.Error,
                Arg.Any<IEnumerable<LoggingProperty>>(),
                Arg.Any<string>(),
                Arg.Any<Exception>());
        }
    }
}
