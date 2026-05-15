namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintRsnDocumentHandlerTests
{
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Exceptions;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenRsnNumberIsNotNumeric : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            var bodyJson = JsonSerializer.Serialize(new
            {
                rsnNumber = "not-a-number",
                copyType = "original",
                facilityCode = "FC001",
                printerUri = "ipp://printer.local:631/ipp/print"
            });

            var message = new Message
                              {
                                  RoutingKey = "print.rsn.document",
                                  Body = Encoding.UTF8.GetBytes(bodyJson)
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
