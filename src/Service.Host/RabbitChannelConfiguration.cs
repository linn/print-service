namespace Linn.PrintService.Service.Host
{
    using System.Threading;
    using System.Threading.Tasks;
    using Linn.Common.Messaging.RabbitMQ;
    using Microsoft.Extensions.Hosting;

    public class RabbitChannelInitializer : IHostedService
    {
        private readonly RabbitChannelConfiguration channelConfig;

        public RabbitChannelInitializer(RabbitChannelConfiguration channelConfig)
        {
            this.channelConfig = channelConfig;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.channelConfig.InitializeAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.channelConfig.DisposeAsync();
        }
    }
}