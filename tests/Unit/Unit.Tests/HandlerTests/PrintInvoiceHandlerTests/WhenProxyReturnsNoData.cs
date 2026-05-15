namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
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

    public class WhenProxyReturnsNoData : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            this.InvoicePrintProxy.GetInvoiceAsPdf(
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<bool>(),
                    Arg.Any<bool>())
                .Returns(new byte[0]);

            var bodyJson = JsonSerializer.Serialize(new
            {
                documentNumber = "12345",
                documentType = "I",
                showTermsAndConditions = false,
                showPrices = true,
                printerUri = "ipp://printer.local:631/ipp/print"
            });

            var message = new Message
                              {
                                  RoutingKey = "print.invoice.document",
                                  Body = Encoding.UTF8.GetBytes(bodyJson)
                              };

            this.action = () => this.Handler.HandleAsync(message, CancellationToken.None);
        }

        [Test]
        public async Task ShouldThrowInvoicePrintMessageException()
        {
            await this.action.Should().ThrowAsync<InvoicePrintMessageException>()
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
