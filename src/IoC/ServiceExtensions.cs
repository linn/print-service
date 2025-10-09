namespace Linn.PrintService.IoC
{
    using System.Net.Http.Headers;
    using System.Text;

    using Linn.Common.Configuration;
    using Linn.Common.Logging;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Printing.Services;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpClient<IIppPrintingService, IppPrintingService>((sp, client) =>
            {
                var log = sp.GetRequiredService<ILog>();

                log.Write(LoggingLevel.Info, null, "Starting IPP HTTP client configuration...");

                var username = ConfigurationManager.Configuration["PRINT_USERNAME"];
                var password = ConfigurationManager.Configuration["PRINT_PASSWORD"];

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    log.Write(LoggingLevel.Error, null, "Missing PRINT_USERNAME or PRINT_PASSWORD configuration.");
                    throw new IppPrintingException("Username and password must be configured.");
                }

                log.Write(LoggingLevel.Info, null, $"PRINT_USERNAME length = {username.Length}, PRINT_PASSWORD length = {password.Length}");

                try
                {
                    var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", authValue);

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/ipp"));

                    var headersLog = string.Join(", ", client.DefaultRequestHeaders.Select(h => $"{h.Key}"));

                    log.Write(LoggingLevel.Info, null, $"Request headers configured: {headersLog}");

                    log.Write(LoggingLevel.Info, null, $"Authorization header set: {client.DefaultRequestHeaders.Authorization != null}");

                    log.Write(LoggingLevel.Info, null, "IPP HTTP client configuration completed successfully.");
                }
                catch (Exception ex)
                {
                    log.Write(LoggingLevel.Error, null, $"Failed to configure IPP HTTP client: {ex.Message}", ex);

                    throw new IppPrintingException("Failed to configure IPP HTTP client.", ex);
                }
            });

            return services;
        }
    }
}
