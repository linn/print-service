namespace Linn.PrintService.Integration.Tests.PrintModuleTests
{
    using System.Net.Http;

    using Linn.Common.Persistence.EntityFramework;
    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Printing;
    using Linn.PrintService.Facade;
    using Linn.PrintService.Facade.ResourceBuilders;
    using Linn.PrintService.IoC;
    using Linn.PrintService.Service.Modules;

    using Microsoft.Extensions.DependencyInjection;

    using NSubstitute;

    using NUnit.Framework;

    public class ContextBase
    {
        protected HttpClient Client { get; private set; }

        protected HttpResponseMessage Response { get; set; }

        protected IIppPrintingService PrintingService { get; private set; }

        protected TestServiceDbContext DbContext { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            this.DbContext = new TestServiceDbContext();

            this.PrintingService = Substitute.For<IIppPrintingService>();

            IPrintFacadeService printFacadeService = new PrintFacadeService(this.PrintingService);

            var repository = new EntityFrameworkQueryRepository<PrinterMapping>(this.DbContext.PrinterMappings);
            var resourceBuilder = new PrinterMappingResourceBuilder();
            IPrinterMappingFacadeService printerMappingFacadeService =
                new PrinterMappingFacadeService(repository, resourceBuilder);

            this.Client = TestClient.With<PrintModule>(
                services =>
                    {
                        services.AddSingleton(printFacadeService);
                        services.AddSingleton(printerMappingFacadeService);
                        services.AddHandlers();
                    });
        }

        [OneTimeTearDown]
        public void TearDownContext()
        {
            this.DbContext.Dispose();
        }
    }
}

