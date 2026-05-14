namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintJobHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingPrintJobMessageWithMissingHeaders : ContextBase
    {
        [SetUp]
        public async Task SetUp()
        {
            var message = new Message
                              {
                                  RoutingKey = "print.job",
                                  Body = new byte[] { 1, 2, 3 },
                                  Headers = new Dictionary<string, object>()
                              };

            await this.Handler.HandleAsync(message, CancellationToken.None);
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
