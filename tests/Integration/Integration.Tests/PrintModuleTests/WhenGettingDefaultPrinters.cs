namespace Linn.PrintService.Integration.Tests.PrintModuleTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using FluentAssertions;

    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Integration.Tests.Extensions;
    using Linn.PrintService.Resources;

    using NUnit.Framework;

    public class WhenGettingDefaultPrinters : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            this.DbContext.PrinterMappings.AddAndSave(
                this.DbContext,
                new PrinterMapping
                {
                    Id = 1,
                    PrinterName = "Service Printer",
                    PrinterGroup = "Service",
                    PrinterUri = "ipp://printer.linn.co.uk/printers/service",
                    PrinterType = "A4",
                    DefaultForGroup = "Y"
                });

            this.DbContext.PrinterMappings.AddAndSave(
                this.DbContext,
                new PrinterMapping
                {
                    Id = 2,
                    PrinterName = "Despatch Printer",
                    PrinterGroup = "Despatch",
                    PrinterUri = "ipp://printer.linn.co.uk/printers/despatch",
                    PrinterType = "A4",
                    DefaultForGroup = "Y"
                });

            this.DbContext.PrinterMappings.AddAndSave(
                this.DbContext,
                new PrinterMapping
                {
                    Id = 3,
                    PrinterName = "Unused Printer",
                    PrinterGroup = "Service",
                    PrinterUri = "ipp://printer.linn.co.uk/printers/unused",
                    PrinterType = "A4",
                    DefaultForGroup = null
                });

            this.Response = this.Client.Get(
                "/print-service/printer-mappings",
                with => { with.Accept("application/json"); }).Result;
        }

        [Test]
        public void ShouldReturnOk()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnOnlyDefaultPrinters()
        {
            var resources = this.Response.DeserializeBody<IEnumerable<PrinterMappingResource>>();
            resources.Should().HaveCount(2);
        }

        [Test]
        public void ShouldNotReturnNonDefaultPrinters()
        {
            var resources = this.Response.DeserializeBody<IEnumerable<PrinterMappingResource>>();
            resources.Should().NotContain(r => r.PrinterName == "Unused Printer");
        }

        [Test]
        public void ShouldReturnCorrectPrinterGroup()
        {
            var resources = this.Response.DeserializeBody<IEnumerable<PrinterMappingResource>>();
            resources.Select(r => r.PrinterGroup).Should().BeEquivalentTo("Service", "Despatch");
        }

        [Test]
        public void ShouldReturnPrinterUri()
        {
            var resources = this.Response.DeserializeBody<IEnumerable<PrinterMappingResource>>();
            resources.First(r => r.PrinterGroup == "Service").PrinterUri
                .Should().Be("ipp://printer.linn.co.uk/printers/service");
        }
    }
}
