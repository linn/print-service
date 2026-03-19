namespace Linn.PrintService.Messaging.Host.Handlers
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Messaging.RabbitMQ;

    public class DebugHandler : IMessageHandler
    {
        public string RoutingKey { get; } = "debug.key";

        public Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            var text = Encoding.UTF8.GetString(message.Body.Span);

            Console.WriteLine($"[DEBUG] Received message:");
            Console.WriteLine($"RoutingKey: {message.RoutingKey}");
            Console.WriteLine($"Body: {text}");

            if (message.Headers.Count > 0)
            {
                Console.WriteLine("Headers:");
                foreach (var kvp in message.Headers)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }

            return Task.CompletedTask;
        }
    }
}