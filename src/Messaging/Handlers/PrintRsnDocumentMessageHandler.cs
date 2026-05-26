namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.Common.Persistence;
    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Domain.LinnApps.Services;
    using Linn.PrintService.Messaging.Exceptions;
    using Linn.PrintService.Messaging.Models;
    using Linn.PrintService.Printing;

    public class PrintRsnDocumentMessageHandler : JsonMessageHandler<PrintRsnDocumentMessageBody>
    {
        private readonly IRsnPrintProxy rsnPrintProxy;
        private readonly IIppPrintingService printingService;
        private readonly IQueryRepository<PrinterMapping> printerMappingRepository;
        private readonly ILog log;

        public PrintRsnDocumentMessageHandler(
            IRsnPrintProxy rsnPrintProxy,
            IIppPrintingService printingService,
            IQueryRepository<PrinterMapping> printerMappingRepository,
            ILog log)
        {
            this.rsnPrintProxy = rsnPrintProxy;
            this.printingService = printingService;
            this.printerMappingRepository = printerMappingRepository;
            this.log = log;
        }

        public override string RoutingKey { get; } = "print.rsn.document";

        public override async Task HandleAsync(
            PrintRsnDocumentMessageBody body,
            IReadOnlyDictionary<string, object> headers,
            CancellationToken cancellationToken)
        {
            this.log.Info("[PrintRsnDocument] Received a message");

            if (body.RsnNumber == 0 || body.CopyType is null || body.FacilityCode is null || body.PrinterGroup is null)
            {
                throw new RsnPrintMessageException(
                    "Missing required field in body: rsnNumber, copyType, facilityCode, or printerGroup");
            }

            var printer = await this.printerMappingRepository.FindByAsync(
                p => p.PrinterGroup == body.PrinterGroup && p.DefaultForGroup == "Y" && p.PrinterType == "A4");

            if (printer == null)
            {
                throw new RsnPrintMessageException(
                    $"No default A4 printer found for group '{body.PrinterGroup}'");
            }

            var jobName = body.JobName ?? $"RSN{body.RsnNumber}";

            this.log.Info(
                $"[PrintRsnDocument] Fetching PDF for RSN {body.RsnNumber}, copyType={body.CopyType}, facilityCode={body.FacilityCode}");

            var data = await this.rsnPrintProxy.GetRsnAsPdf(body.RsnNumber, body.CopyType, body.FacilityCode);

            if (data == null || data.Length == 0)
            {
                throw new RsnPrintMessageException($"No PDF data returned for RSN {body.RsnNumber}");
            }

            this.log.Info($"[PrintRsnDocument] Received {data.Length} bytes, printing to {printer.PrinterUri}");

            await this.printingService.Print(printer.PrinterUri, jobName, data);

            this.log.Info($"[PrintRsnDocument] Print job completed: {jobName}");
        }
    }
}
