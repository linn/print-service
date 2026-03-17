namespace Linn.PrintService.IoC
{
    using System.Net.Http.Headers;
    using System.Text;

    using Linn.Common.Configuration;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Handlers;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Printing.Services;

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

            return services;
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
                new RabbitChannelConfiguration(
                    queueName: "print.queue",
                    routingKeys: new[] { "print.job" },
                    exchangeName: "print.exchange",
                    durableExchange: true
                ));

            services.AddSingleton<RabbitPublisher>(sp =>
                {
                    var config = sp.GetRequiredService<RabbitChannelConfiguration>();
                    return new RabbitPublisher(config.ProducerChannel!, config.Exchange!);
                });

            services.AddSingleton<IMessageHandler, PrintJobMessageHandler>();

            return services;
        }
    }
}