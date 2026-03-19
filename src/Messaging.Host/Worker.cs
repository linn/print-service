namespace Linn.PrintService.Messaging.Host
{
    using Linn.Common.Messaging.RabbitMQ;

    public class Worker : BackgroundService
    {
        private readonly RabbitChannelConfiguration channelConfig;
        private readonly IEnumerable<IMessageHandler> handlers;

        public Worker(
            RabbitChannelConfiguration channelConfig,
            IEnumerable<IMessageHandler> handlers)
        {
            this.channelConfig = channelConfig;
            this.handlers = handlers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"[Worker] Starting... Queue={this.channelConfig.QueueName}");

            var channel = this.channelConfig.ConsumerChannel;
            var queueName = this.channelConfig.QueueName;

            if (channel == null)
            {
                Console.WriteLine("[Worker] ConsumerChannel not initialized!");
                throw new Exception("ConsumerChannel not initialized");
            }

            var router = new RabbitMessageRouter(channel, this.handlers);
            var consumer = router.CreateConsumer(stoppingToken);

            Console.WriteLine("[Worker] Subscribing to queue...");
            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer,
                cancellationToken: stoppingToken);

            Console.WriteLine("[Worker] Print worker running. Waiting for messages...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}