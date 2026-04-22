namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintPackingListHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Exceptions;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenConsignmentNumberIsNotNumeric : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            var message = new Message
                              {
                                  RoutingKey = "print.packing-list.document",
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "consignmentNumber", Encoding.UTF8.GetBytes("not-a-number") },
                                                    { "printerUri", Encoding.UTF8.GetBytes("ipp://printer.local:631/ipp/print") }
                                                }
                              };

            this.action = () => this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public async Task ShouldThrowRsnPrintMessageException()
        {
            await this.action.Should().ThrowAsync<PackingListPrintMessageException>()
                .WithMessage("*not-a-number*");
        }

        [Test]
        public void ShouldNotCallProxy()
        {
            this.PackingListProxy.DidNotReceive().GetPackingListAsPdf(Arg.Any<int>());
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
