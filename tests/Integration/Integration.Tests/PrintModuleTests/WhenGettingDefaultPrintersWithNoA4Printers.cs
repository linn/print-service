namespace Linn.PrintService.Integration.Tests.PrintModuleTests
{
    using System.Net;

    using FluentAssertions;

    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Integration.Tests.Extensions;

    using NUnit.Framework;

    public class WhenGettingDefaultPrintersWithNoA4Printers : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            this.DbContext.PrinterMappings.AddAndSave(
                this.DbContext,
                new PrinterMapping
                {
                    Id = 1,
                    PrinterName = "Label Printer",
                    PrinterGroup = "Service",
                    PrinterUri = "ipp://printer.linn.co.uk/printers/label",
                    PrinterType = "LABEL",
                    DefaultForGroup = "Y"
                });

            this.Response = this.Client.Get(
                "/print-service/printer-mappings",
                with => { with.Accept("application/json"); }).Result;
        }

        [Test]
        public void ShouldReturnBadRequest()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
