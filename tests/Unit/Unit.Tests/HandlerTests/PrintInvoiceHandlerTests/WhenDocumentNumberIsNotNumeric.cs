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
    using Linn.PrintService.Messaging.Models;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenDocumentNumberIsNotNumeric : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            var bodyJson = JsonSerializer.Serialize(new PrintInvoiceMessageBody
            {
                DocumentNumber = "not-a-number",
                DocumentType = "I",
                PrinterUri = "ipp://printer.local:631/ipp/print"
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
                .WithMessage("*not-a-number*");
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
