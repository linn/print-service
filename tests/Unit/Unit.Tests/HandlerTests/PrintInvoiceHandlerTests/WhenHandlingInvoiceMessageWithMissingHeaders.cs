namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Exceptions;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenHandlingInvoiceMessageWithMissingHeaders : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            var message = new Message
                              {
                                  RoutingKey = "print.invoice.document",
                                  Headers = new Dictionary<string, object>()
                              };

            this.action = () => this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public async Task ShouldThrowInvoicePrintMessageException()
        {
            await this.action.Should().ThrowAsync<InvoicePrintMessageException>()
                .WithMessage("*Missing required header*");
        }

        [Test]
        public void ShouldNotCallProxy()
        {
            this.InvoicePrintProxy.DidNotReceive().GetInvoiceAsPdf(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<bool>(),
                Arg.Any<bool>());
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
