namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintPackingListHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Host.Exceptions;


    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingPackingListMessageWithMissingHeaders : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            var message = new Message
                              {
                                  RoutingKey = "print.packing-list",
                                  Headers = new Dictionary<string, object>()
                              };

            this.action = () => this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public async Task ShouldThrowRsnPrintMessageException()
        {
            await this.action.Should().ThrowAsync<PackingListPrintMessageException>()
                .WithMessage("*Missing required header*");
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
