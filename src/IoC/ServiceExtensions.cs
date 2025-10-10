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

                log.Write(LoggingLevel.Info, null, $"PRINT_USERNAME = {username}, PRINT_PASSWORD length = {password.Length}");

                try
                {
                    var rawAuthString = $"{username}:{password}";
                    log.Write(LoggingLevel.Info, null, $"Raw auth string length: {rawAuthString.Length}");

                    var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawAuthString));
                    log.Write(LoggingLevel.Info, null, $"Encoded auth length: {authValue.Length}");

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", authValue);

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/ipp"));

                    var authHeader = client.DefaultRequestHeaders.Authorization?.Parameter;
                    var maskedAuth = authHeader != null && authHeader.Length > 8
                        ? authHeader.Substring(0, 8) + "...(truncated)"
                        : authHeader ?? "(null)";
                   
                    log.Write(LoggingLevel.Info, null, $"Authorization header value (masked): {maskedAuth}");

                    log.Write(LoggingLevel.Info, null, $"Authorization header set: {client.DefaultRequestHeaders.Authorization != null}");

                    foreach (var header in client.DefaultRequestHeaders)
                    {
                        log.Write(LoggingLevel.Info, null, $"Header: {header.Key} = {string.Join(", ", header.Value)}");
                    }

                    if (client.BaseAddress != null)
                    {
                        log.Write(LoggingLevel.Info, null, $"IPP request target URI: {client.BaseAddress}");
                        log.Write(LoggingLevel.Info, null, $"IPP request target host: {client.BaseAddress.Host}");
                    }
                    else
                    {
                        log.Write(LoggingLevel.Info, null, "BaseAddress not set on IPP HTTP client.");
                    }

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
