namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintPackingListHandlerTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

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
            this.PackingListProxy.GetPackingListAsPdf(Arg.Any<int>())
                .Returns(new byte[0]);

            this.action = () => this.Handler.HandleAsync(
                new PrintPackingListMessageBody
                    {
                        ConsignmentId = 67890,
                        PrinterUri = "ipp://printer.local:631/ipp/print"
                    },
                new Dictionary<string, object>(),
                CancellationToken.None);
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
