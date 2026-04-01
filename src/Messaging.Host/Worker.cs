namespace Linn.PrintService.Messaging.Host
{
    using Linn.Common.Messaging.RabbitMQ;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Worker : BackgroundService
    {
        private readonly RabbitChannelConfiguration config;
        private readonly IEnumerable<IMessageHandler> handlers;
        private readonly ILogger<Worker> logger;

        public Worker(
            RabbitChannelConfiguration config,
            IEnumerable<IMessageHandler> handlers,
            ILogger<Worker> logger)
        {
            this.config = config;
            this.handlers = handlers;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Worker : Starting... Queue={Queue}", this.config.QueueName);

            while (this.config.ConsumerChannel == null && !stoppingToken.IsCancellationRequested)
            {
                this.logger.LogInformation("[Worker : Waiting for ConsumerChannel...");
                await Task.Delay(200, stoppingToken);
            }

            var channel = this.config.ConsumerChannel;
            var router = new RabbitMessageRouter(channel, this.handlers);
            var consumer = router.CreateConsumer(stoppingToken);

            this.logger.LogInformation("[Worker] Subscribing to queue...");

            await channel.BasicConsumeAsync(
                queue: this.config.QueueName,
                autoAck: false,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer,
                cancellationToken: stoppingToken);

            this.logger.LogInformation("[Worker] Running");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
