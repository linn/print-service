namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Models;
    using Linn.PrintService.Printing;

    public class PrintJobMessageHandler : JsonMessageHandler<PrintJobMessageBody>
    {
        private readonly IIppPrintingService printingService;
        private readonly ILog log;

        public PrintJobMessageHandler(IIppPrintingService printingService, ILog log)
        {
            this.printingService = printingService;
            this.log = log;
        }

        public override string RoutingKey { get; } = "print.job";

        protected override async Task HandleAsync(
            PrintJobMessageBody body,
            IReadOnlyDictionary<string, object> headers,
            CancellationToken cancellationToken)
        {
            this.log.Info("[Handler] Received a message");

            try
            {
                if (body.PrinterUri is null)
                {
                    throw new IppPrintingException("Missing printerUri in message body");
                }

                var jobName = body.JobName ?? "PrintJob";
                var data = body.Data ?? Array.Empty<byte>();

                this.log.Info($"[Handler] Processing print job: {jobName} for {body.PrinterUri}, {data.Length} bytes");

                await this.printingService.Print(body.PrinterUri, jobName, data);

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
