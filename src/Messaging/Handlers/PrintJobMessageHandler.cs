namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Extensions;
    using Linn.PrintService.Messaging.Models;
    using Linn.PrintService.Printing;

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
                var body = message.DeserializeBody<PrintJobMessageBody>();

                if (body?.PrinterUri is null || body.JobName is null)
                {
                    throw new IppPrintingException("Missing printerUri or jobName in message body");
                }

                var printerUri = body.PrinterUri;
                var jobName = body.JobName;
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
