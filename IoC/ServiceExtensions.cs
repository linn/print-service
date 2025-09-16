namespace Linn.PrintService.IoC
{
    using System.Net;
    using Linn.Common.Configuration;
    using Linn.PrintService.Printing.Services;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddSingleton<IPrintingService, PrintingService>();
        }
    }
}
