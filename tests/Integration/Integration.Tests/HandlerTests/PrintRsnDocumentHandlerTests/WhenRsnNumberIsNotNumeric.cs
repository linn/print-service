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

    public class WhenRsnNumberIsNotNumeric : ContextBase
    {
        [SetUp]
        public async Task SetUp()
        {
            var message = new Message
                              {
                                  RoutingKey = "print.rsn.document",
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "rsnNumber", Encoding.UTF8.GetBytes("not-a-number") },
                                                    { "copyType", Encoding.UTF8.GetBytes("original") },
                                                    { "facilityCode", Encoding.UTF8.GetBytes("FC001") },
                                                    { "printerUri", Encoding.UTF8.GetBytes("ipp://printer.local:631/ipp/print") }
                                                }
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
        public void ShouldLogAsPrintingError()
        {
            this.Log.Received(1).Write(
                LoggingLevel.Error,
                Arg.Any<IEnumerable<LoggingProperty>>(),
                Arg.Is<string>(s => s.Contains("IPP printing failed") && s.Contains("not-a-number")),
                Arg.Any<Exception>());
        }
    }
}
