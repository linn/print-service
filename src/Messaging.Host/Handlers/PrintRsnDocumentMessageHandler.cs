namespace Linn.PrintService.Messaging.Host.Handlers
{
    using System.Text;

    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Printing.Services;

    public class PrintRsnDocumentMessageHandler : IMessageHandler
    {
        private readonly IRsnPrintProxy rsnPrintProxy;
        private readonly IIppPrintingService printingService;
        private readonly ILog log;

        public PrintRsnDocumentMessageHandler(
            IRsnPrintProxy rsnPrintProxy,
            IIppPrintingService printingService,
            ILog log)
        {
            this.rsnPrintProxy = rsnPrintProxy;
            this.printingService = printingService;
            this.log = log;
        }

        public string RoutingKey { get; } = "print.rsn.document";

        public async Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            this.log.Info("[PrintRsnDocument] Received a message");

            try
            {
                if (!message.Headers.TryGetValue("rsnNumber", out var rsnNumberObj)
                    || !message.Headers.TryGetValue("copyType", out var copyTypeObj)
                    || !message.Headers.TryGetValue("facilityCode", out var facilityCodeObj)
                    || !message.Headers.TryGetValue("printerUri", out var printerUriObj))
                {
                    throw new IppPrintingException(
                        "Missing required header: rsnNumber, copyType, facilityCode, or printerUri");
                }

                var rsnNumber = Encoding.UTF8.GetString((byte[])rsnNumberObj);
                var copyType = Encoding.UTF8.GetString((byte[])copyTypeObj);
                var facilityCode = Encoding.UTF8.GetString((byte[])facilityCodeObj);
                var printerUri = Encoding.UTF8.GetString((byte[])printerUriObj);

                var jobName = message.Headers.TryGetValue("jobName", out var jobNameObj)
                    ? Encoding.UTF8.GetString((byte[])jobNameObj)
                    : $"RSN{rsnNumber}";

                this.log.Info(
                    $"[PrintRsnDocument] Fetching PDF for RSN {rsnNumber}, copyType={copyType}, facilityCode={facilityCode}");

                var data = await this.rsnPrintProxy.GetRsnPrintAsPdf(
                    int.Parse(rsnNumber),
                    copyType,
                    facilityCode);

                this.log.Info(
                    $"[PrintRsnDocument] Received {data.Length} bytes, printing to {printerUri}");

                await this.printingService.Print(printerUri, jobName, data);

                this.log.Info($"[PrintRsnDocument] Print job completed: {jobName}");
            }
            catch (IppPrintingException ex)
            {
                this.log.Error($"[PrintRsnDocument] IPP printing failed: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                this.log.Error($"[PrintRsnDocument] Handler exception: {ex.Message}", ex);
            }
        }
    }
}
