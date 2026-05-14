namespace Linn.PrintService.IoC
{
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Handlers;

    using Microsoft.Extensions.DependencyInjection;

    public static class MessagingExtensions
    {
        public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IMessageHandler, PrintJobMessageHandler>();
            services.AddSingleton<IMessageHandler, PrintRsnDocumentMessageHandler>();
            services.AddSingleton<IMessageHandler, PrintPackingListMessageHandler>();
            services.AddSingleton<IMessageHandler, PrintInvoiceMessageHandler>();

            return services;
        }
    }
}
