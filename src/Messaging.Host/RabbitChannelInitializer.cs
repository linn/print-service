namespace Linn.PrintService.Messaging.Host
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;

    public class RabbitChannelInitializer : IHostedService
    {
        private readonly RabbitChannelConfiguration config;
        private readonly ILog log;

        public RabbitChannelInitializer(
            RabbitChannelConfiguration config,
            ILog log)
        {
            this.config = config;
            this.log = log;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.log.Info("[Initializer] Starting RabbitMQ initialization...");

            try
            {
                await this.config.InitializeAsync(cancellationToken);

                this.log.Info(
                    $"[Initializer] Initialized. Queue={this.config.QueueName}, Exchange={this.config.Exchange}");
            }
            catch (Exception ex)
            {
                this.log.Error("[Initializer] Failed to initialize RabbitMQ channel.", ex);
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.config.DisposeAsync();
        }
    }
}