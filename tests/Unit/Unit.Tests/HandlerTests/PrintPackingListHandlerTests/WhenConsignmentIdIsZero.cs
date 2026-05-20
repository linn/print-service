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

    public class WhenConsignmentIdIsZero : ContextBase
    {
        private Func<Task> action;

        [SetUp]
        public void SetUp()
        {
            this.action = () => this.Handler.HandleAsync(
                new PrintPackingListMessageBody
                    {
                        ConsignmentId = 0,
                        PrinterUri = "ipp://printer.local:631/ipp/print"
                    },
                new Dictionary<string, object>(),
                CancellationToken.None);
        }

        [Test]
        public async Task ShouldThrowPackingListPrintMessageException()
        {
            await this.action.Should().ThrowAsync<PackingListPrintMessageException>()
                .WithMessage("*Missing required field*");
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
