namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
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
            this.InvoicePrintProxy.GetInvoiceAsPdf(
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<bool>(),
                    Arg.Any<bool>())
                .Returns(new byte[0]);

            var message = new Message
                              {
                                  RoutingKey = "print.invoice.document",
                                  Headers = new Dictionary<string, object>
                                                {
                                                    { "documentNumber", Encoding.UTF8.GetBytes("12345") },
                                                    { "documentType", Encoding.UTF8.GetBytes("INV") },
                                                    { "showTermsAndConditions", Encoding.UTF8.GetBytes("false") },
                                                    { "showPrices", Encoding.UTF8.GetBytes("true") },
                                                    { "printerUri", Encoding.UTF8.GetBytes("ipp://printer.local:631/ipp/print") }
                                                }
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
