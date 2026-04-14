namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintRsnDocumentHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Host.Exceptions;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenRsnNumberIsNotNumeric : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
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

            this.action = () => this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public async Task ShouldThrowRsnPrintMessageException()
        {
            await this.action.Should().ThrowAsync<RsnPrintMessageException>()
                .WithMessage("*not-a-number*");
        }

        [Test]
        public void ShouldNotCallProxy()
        {
            this.RsnPrintProxy.DidNotReceive().GetRsnAsPdf(
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
    }
}
