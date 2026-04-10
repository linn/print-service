namespace Linn.PrintService.Messaging.Host.Handlers
{
    using System.Text;

    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Printing.Services;

    public class PrintJobMessageHandler : IMessageHandler
    {
        private readonly IIppPrintingService printingService;
        private readonly ILog log;

        public PrintJobMessageHandler(IIppPrintingService printingService, ILog log)
        {
            this.printingService = printingService;
            this.log = log;
        }

        public string RoutingKey { get; } = "print.job";

        public async Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            this.log.Info("[Handler] Received a message");

            try
            {
                if (!message.Headers.TryGetValue("printerUri", out var printerUriObj) ||
                    !message.Headers.TryGetValue("jobName", out var jobNameObj))
                {
                    throw new IppPrintingException("Missing printerUri or jobName header");
                }

                var printerUri = Encoding.UTF8.GetString((byte[])printerUriObj);
                var jobName = Encoding.UTF8.GetString((byte[])jobNameObj) ?? "PrintJob";
                var data = message.Body.ToArray();

                this.log.Info($"[Handler] Processing print job: {jobName} for {printerUri}, {data.Length} bytes");

                await this.printingService.Print(printerUri, jobName, data);

                this.log.Info($"[Handler] Print job completed: {jobName}");
            }
            catch (IppPrintingException ex)
            {
                this.log.Error($"[Handler] IPP printing failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                this.log.Error($"[Handler] Handler exception: {ex.Message}", ex);
            }
        }
    }
}