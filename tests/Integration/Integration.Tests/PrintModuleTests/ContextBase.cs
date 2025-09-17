namespace Linn.PrintService.Integration.Tests.PrintModuleTests
{
    using System.Net.Http;

    using Linn.PrintService.Printing.Services;
    using Linn.PrintService.Service.Modules;

    using Microsoft.Extensions.DependencyInjection;

    using NSubstitute;

    using NUnit.Framework;

    public class ContextBase
    {
        protected HttpClient Client { get; private set; }

        protected HttpResponseMessage Response { get; set; }

        protected IPrintingService PrintingService { get; private set; }

        [SetUp]
        public void SetUpContext()
        {
            this.PrintingService = Substitute.For<IPrintingService>();

            this.Client = TestClient.With<PrintModule>(
                services =>
                    {
                        services.AddSingleton(this.PrintingService);
                        services.AddRouting();
                    });
        }
    }
}