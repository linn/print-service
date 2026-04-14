namespace Linn.PrintService.Integration.Tests.HandlerTests.PrintRsnDocumentHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingRsnDocumentMessageWithMissingHeaders : ContextBase
    {
        [SetUp]
        public async Task SetUp()
        {
            var message = new Message
                              {
                                  RoutingKey = "print.rsn.document",
                                  Headers = new Dictionary<string, object>()
                              };

            await this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public void ShouldNotCallProxy()
        {
            this.RsnPrintProxy.DidNotReceive().GetRsnPrintAsPdf(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>());
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
