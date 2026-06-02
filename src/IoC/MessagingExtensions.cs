namespace Linn.PrintService.IoC
{
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Handlers;

    using Microsoft.Extensions.DependencyInjection;

    public static class MessagingExtensions
    {
        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.AddScoped<IMessageHandler, PrintJobMessageHandler>();
            services.AddScoped<IMessageHandler, PrintRsnDocumentMessageHandler>();
            services.AddScoped<IMessageHandler, PrintPackingListMessageHandler>();
            services.AddScoped<IMessageHandler, PrintInvoiceMessageHandler>();

            return services;
        }
    }
}
