namespace Linn.PrintService.Unit.Tests.HandlerTests.PrintRsnDocumentHandlerTests
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
            this.RsnPrintProxy.GetRsnAsPdf(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new byte[0]);

            this.PrinterMappingRepository
                .FindByAsync(Arg.Any<System.Linq.Expressions.Expression<System.Func<Linn.PrintService.Domain.LinnApps.PrinterMapping, bool>>>())
                .Returns(new Linn.PrintService.Domain.LinnApps.PrinterMapping
                    {
                        PrinterGroup = "GROUP1",
                        PrinterUri = "ipp://printer.local:631/ipp/print",
                        PrinterType = "A4",
                        DefaultForGroup = "Y"
                    });

            this.action = () => this.Handler.HandleAsync(
                new PrintRsnDocumentMessageBody
                    {
                        RsnNumber = 12345,
                        CopyType = "service",
                        FacilityCode = "FC001",
                        PrinterGroup = "GROUP1"
                    },
                new Dictionary<string, object>(),
                CancellationToken.None);
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
