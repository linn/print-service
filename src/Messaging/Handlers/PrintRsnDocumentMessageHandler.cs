namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Domain.LinnApps.Services;
    using Linn.PrintService.Messaging.Exceptions;
    using Linn.PrintService.Messaging.Models;
    using Linn.PrintService.Printing;

    public class PrintRsnDocumentMessageHandler : JsonMessageHandler<PrintRsnDocumentMessageBody>
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

        public override string RoutingKey { get; } = "print.rsn.document";

        protected override async Task HandleAsync(
            PrintRsnDocumentMessageBody body,
            IReadOnlyDictionary<string, object> headers,
            CancellationToken cancellationToken)
        {
            this.log.Info("[PrintRsnDocument] Received a message");

            if (body.RsnNumber is null || body.CopyType is null || body.FacilityCode is null || body.PrinterUri is null)
            {
                throw new RsnPrintMessageException(
                    "Missing required field in body: rsnNumber, copyType, facilityCode, or printerUri");
            }

            if (!int.TryParse(body.RsnNumber, out var parsedRsnNumber))
            {
                throw new RsnPrintMessageException(
                    $"Invalid rsnNumber value: '{body.RsnNumber}' is not a valid integer");
            }

            var jobName = body.JobName ?? $"RSN{body.RsnNumber}";

            this.log.Info(
                $"[PrintRsnDocument] Fetching PDF for RSN {body.RsnNumber}, copyType={body.CopyType}, facilityCode={body.FacilityCode}");

            var data = await this.rsnPrintProxy.GetRsnAsPdf(parsedRsnNumber, body.CopyType, body.FacilityCode);

            if (data == null || data.Length == 0)
            {
                throw new RsnPrintMessageException($"No PDF data returned for RSN {body.RsnNumber}");
            }

            this.log.Info($"[PrintRsnDocument] Received {data.Length} bytes, printing to {body.PrinterUri}");

            await this.printingService.Print(body.PrinterUri, jobName, data);

            this.log.Info($"[PrintRsnDocument] Print job completed: {jobName}");
        }
    }
}
