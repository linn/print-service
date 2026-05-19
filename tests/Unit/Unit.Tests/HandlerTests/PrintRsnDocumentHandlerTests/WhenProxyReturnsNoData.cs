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
    using Linn.PrintService.Messaging.Models;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenProxyReturnsNoData : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            this.RsnPrintProxy.GetRsnAsPdf(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new byte[0]);

            var bodyJson = JsonSerializer.Serialize(new PrintRsnDocumentMessageBody
            {
                RsnNumber = "12345",
                CopyType = "service",
                FacilityCode = "FC001",
                PrinterUri = "ipp://printer.local:631/ipp/print"
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
