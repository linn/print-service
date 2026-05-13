namespace Linn.PrintService.IoC
{
    using System.Net.Http.Headers;
    using System.Text;

    using Linn.Common.Configuration;
    using Linn.Common.Facade;
    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Domain.LinnApps.Services;
    using Linn.PrintService.Facade;
    using Linn.PrintService.Facade.ResourceBuilders;
    using Linn.PrintService.Proxy;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpClient<IIppPrintingService, IppPrintingService>(client =>
                {
                    var username = ConfigurationManager.Configuration["PRINT_USERNAME"];
                    var password = ConfigurationManager.Configuration["PRINT_PASSWORD"];

                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                    {
                        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Basic", authValue);
                    }

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/ipp"));
                });

            services.AddHttpClient<IRsnPrintProxy, RsnPrintProxy>(client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/pdf"));
                });

            services.AddHttpClient<IPackingListProxy, PackingListProxy>(client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/pdf"));
                });

            services.AddHttpClient<IInvoicePrintProxy, InvoicePrintProxy>(client =>
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/pdf"));
                });

            return services;
        }

        public static IServiceCollection AddFacadeServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IPrintFacadeService, PrintFacadeService>()
                .AddScoped<IPrinterMappingFacadeService, PrinterMappingFacadeService>();
        }

        public static IServiceCollection AddBuilders(this IServiceCollection services)
        {
            return services
                .AddScoped<IBuilder<PrinterMapping>, PrinterMappingResourceBuilder>();
        }
    }
}
