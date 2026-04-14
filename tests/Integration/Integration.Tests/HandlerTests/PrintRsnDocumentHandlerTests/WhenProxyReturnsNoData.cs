namespace Linn.PrintService.Integration.Tests.HandlerTests.PrintRsnDocumentHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenProxyReturnsNoData : ContextBase
    {
        [SetUp]
        public async Task SetUp()
        {
            this.RsnPrintProxy.GetRsnPrintAsPdf(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new byte[0]);

            var message = new Message
                              {
                                  RoutingKey = "print.rsn.document",
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "rsnNumber", Encoding.UTF8.GetBytes("12345") },
                                                    { "copyType", Encoding.UTF8.GetBytes("original") },
                                                    { "facilityCode", Encoding.UTF8.GetBytes("FC001") },
                                                    { "printerUri", Encoding.UTF8.GetBytes("ipp://printer.local:631/ipp/print") }
                                                }
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
