namespace Linn.PrintService.Messaging.Host
{
    using Linn.Common.Messaging.RabbitMQ;

    public class Worker : BackgroundService
    {
        private readonly IEnumerable<IMessageHandler> handlers;

        public Worker(IEnumerable<IMessageHandler> handlers)
        {
            this.handlers = handlers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // configure debug exchange/queue/DLQ via common configurator
            var channelConfig = new RabbitChannelConfiguration(
                queueName: "print.queue",
                routingKeys: new[] { "print.job" },
                exchangeName: "print.exchange",
                durableExchange: true,
                createConsumerChannel: true,
                createProducerChannel: true);

            await channelConfig.InitializeAsync(stoppingToken);

            var channel = channelConfig.ConsumerChannel;
            var queueName = $"{channelConfig.QueueName}";

            // setup router
            if (channel != null)
            {
                var router = new RabbitMessageRouter(channel, this.handlers);
                var consumer = router.CreateConsumer(stoppingToken);

                await channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumerTag: string.Empty,
                    noLocal: false,
                    exclusive: false,
                    arguments: null,
                    consumer: consumer,
                    cancellationToken: stoppingToken);
            }

            Console.WriteLine("RabbitMQ DEBUG consumer running...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}