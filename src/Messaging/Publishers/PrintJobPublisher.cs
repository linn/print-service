namespace Linn.PrintService.Messaging.Publishers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Linn.Common.Messaging.RabbitMQ;

    public class PrintJobPublisher
    {
        private readonly RabbitPublisher rabbitPublisher;
        private readonly string routingKey;
        private readonly IReadOnlyDictionary<string, object>? defaultHeaders;

        public PrintJobPublisher(
            RabbitPublisher rabbitPublisher,
            string routingKey = "print.job",
            IReadOnlyDictionary<string, object>? defaultHeaders = null)
        {
            this.rabbitPublisher = rabbitPublisher;
            this.routingKey = routingKey;
            this.defaultHeaders = defaultHeaders;
        }

        public async Task PublishAsync(byte[] data, string printerUri, string jobName, CancellationToken cancellationToken = default)
        {
            var headers = new Dictionary<string, object>(this.defaultHeaders ?? new Dictionary<string, object>())
                              {
                                  { "printerUri", printerUri },
                                  { "jobName", jobName }
                              };

            var message = new Message
                              {
                                  RoutingKey = this.routingKey,
                                  Body = data,
                                  Headers = headers
                              };

            await this.rabbitPublisher.PublishAsync(message, cancellationToken);
        }
    }
}