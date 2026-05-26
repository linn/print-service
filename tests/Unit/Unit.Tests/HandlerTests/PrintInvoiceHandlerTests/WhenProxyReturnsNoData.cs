namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintInvoiceHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.PrintService.Domain.LinnApps;
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
            this.InvoicePrintProxy.GetInvoiceAsPdf(
                    Arg.Any<string>(),
                    Arg.Any<int>(),
                    Arg.Any<bool>(),
                    Arg.Any<bool>())
                .Returns(new byte[0]);

            this.PrinterMappingRepository
                .FindByAsync(Arg.Any<Expression<Func<PrinterMapping, bool>>>())
                .Returns(new PrinterMapping
                    {
                        PrinterGroup = "ACCOUNTS",
                        PrinterUri = "ipp://printer.local:631/ipp/print",
                        PrinterType = "A4",
                        DefaultForGroup = "Y"
                    });

            this.action = () => this.Handler.HandleAsync(
                new PrintInvoiceMessageBody
                    {
                        DocumentNumber = 12345,
                        DocumentType = "I",
                        ShowTermsAndConditions = false,
                        ShowPrices = true,
                        PrinterGroup = "ACCOUNTS"
                    },
                new Dictionary<string, object>(),
                CancellationToken.None);
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
