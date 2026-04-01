namespace Linn.PrintService.Messaging.Host
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Linn.Common.Messaging.RabbitMQ;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class RabbitChannelInitializer : IHostedService
    {
        private readonly RabbitChannelConfiguration config;
        private readonly ILogger<RabbitChannelInitializer> logger;

        public RabbitChannelInitializer(
            RabbitChannelConfiguration config,
            ILogger<RabbitChannelInitializer> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("[Initializer] Starting RabbitMQ initialization...");

            try
            {
                await this.config.InitializeAsync(cancellationToken);

                this.logger.LogInformation(
                    "[Initializer] Initialized. Queue={Queue}, Exchange={Exchange}",
                    this.config.QueueName,
                    this.config.Exchange);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "[Initializer] Failed to initialize RabbitMQ channel.");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.config.DisposeAsync();
        }
    }
}