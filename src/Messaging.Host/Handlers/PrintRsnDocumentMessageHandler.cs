namespace Linn.PrintService.Messaging.Host.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Host.Exceptions;
    using Linn.PrintService.Messaging.Host.Extensions;
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

            if (!message.TryGetHeaderAsString("rsnNumber", out var rsnNumber)
                || !message.TryGetHeaderAsString("copyType", out var copyType)
                || !message.TryGetHeaderAsString("facilityCode", out var facilityCode)
                || !message.TryGetHeaderAsString("printerUri", out var printerUri))
            {
                throw new RsnPrintMessageException(
                    "Missing required header: rsnNumber, copyType, facilityCode, or printerUri");
            }

            if (!int.TryParse(rsnNumber, out var parsedRsnNumber))
            {
                throw new RsnPrintMessageException(
                    $"Invalid rsnNumber header value: '{rsnNumber}' is not a valid integer");
            }

            var jobName = message.TryGetHeaderAsString("jobName", out var jobNameValue)
                ? jobNameValue
                : $"RSN{rsnNumber}";

            this.log.Info(
                $"[PrintRsnDocument] Fetching PDF for RSN {rsnNumber}, copyType={copyType}, facilityCode={facilityCode}");

            var data = await this.rsnPrintProxy.GetRsnAsPdf(
                parsedRsnNumber,
                copyType,
                facilityCode);

            if (data == null || data.Length == 0)
            {
                throw new RsnPrintMessageException(
                    $"No PDF data returned for RSN {rsnNumber}");
            }

            this.log.Info(
                $"[PrintRsnDocument] Received {data.Length} bytes, printing to {printerUri}");

            await this.printingService.Print(printerUri, jobName, data);

            this.log.Info($"[PrintRsnDocument] Print job completed: {jobName}");
        }
    }
}
