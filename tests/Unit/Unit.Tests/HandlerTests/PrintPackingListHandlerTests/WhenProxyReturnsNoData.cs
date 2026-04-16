namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintPackingListHandlerTests
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

    public class WhenProxyReturnsNoData : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            this.PackingListProxy.GetPackingListAsPdf(Arg.Any<int>())
                .Returns(new byte[0]);

            var message = new Message
                              {
                                  RoutingKey = "print.packing-list",
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "consignmentNumber", Encoding.UTF8.GetBytes("67890") },
                                                    { "printerUri", Encoding.UTF8.GetBytes("ipp://printer.local:631/ipp/print") }
                                                }
                              };

            this.action = () => this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public async Task ShouldThrowRsnPrintMessageException()
        {
            await this.action.Should().ThrowAsync<PackingListPrintMessageException>()
                .WithMessage("*No PDF data returned*");
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
