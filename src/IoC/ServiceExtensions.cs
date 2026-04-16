namespace Linn.PrintService.IoC
{
    using System.Net.Http.Headers;
    using System.Text;

    using Linn.Common.Configuration;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Printing.Services;
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

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        throw new IppPrintingException(
                            "Username and password must be configured.");
                    }

                    var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", authValue);

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

            return services;
        }
    }
}