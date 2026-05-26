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

    public class PrintInvoiceMessageHandler : JsonMessageHandler<PrintInvoiceMessageBody>
    {
        private readonly IInvoicePrintProxy invoicePrintProxy;
        private readonly IIppPrintingService printingService;
        private readonly IQueryRepository<PrinterMapping> printerMappingRepository;
        private readonly ILog log;

        public PrintInvoiceMessageHandler(
            IInvoicePrintProxy invoicePrintProxy,
            IIppPrintingService printingService,
            IQueryRepository<PrinterMapping> printerMappingRepository,
            ILog log)
        {
            this.invoicePrintProxy = invoicePrintProxy;
            this.printingService = printingService;
            this.printerMappingRepository = printerMappingRepository;
            this.log = log;
        }

        public override string RoutingKey { get; } = "print.invoice.document";

        public override async Task HandleAsync(
            PrintInvoiceMessageBody body,
            IReadOnlyDictionary<string, object> headers,
            CancellationToken cancellationToken)
        {
            this.log.Info("[PrintInvoice] Received a message");

            if (body.DocumentNumber == 0 || body.DocumentType is null || body.PrinterGroup is null)
            {
                throw new InvoicePrintMessageException(
                    "Missing required field in body: documentNumber, documentType, or printerGroup");
            }

            var printer = await this.printerMappingRepository.FindByAsync(
                p => p.PrinterGroup == body.PrinterGroup && p.DefaultForGroup == "Y" && p.PrinterType == "A4");

            if (printer == null)
            {
                throw new InvoicePrintMessageException(
                    $"No default A4 printer found for group '{body.PrinterGroup}'");
            }

            var jobName = body.JobName ?? $"Invoice_{body.DocumentNumber}";

            this.log.Info(
                $"[PrintInvoice] Fetching PDF for {body.DocumentType} {body.DocumentNumber}, showTerms={body.ShowTermsAndConditions}, showPrices={body.ShowPrices}");

            var data = await this.invoicePrintProxy.GetInvoiceAsPdf(
                body.DocumentType,
                body.DocumentNumber,
                body.ShowTermsAndConditions,
                body.ShowPrices);

            if (data == null || data.Length == 0)
            {
                throw new InvoicePrintMessageException(
                    $"No PDF data returned for {body.DocumentType} {body.DocumentNumber}");
            }

            this.log.Info($"[PrintInvoice] Received {data.Length} bytes, printing to {printer.PrinterUri}");

            await this.printingService.Print(printer.PrinterUri, jobName, data);

            this.log.Info($"[PrintInvoice] Print job completed: {jobName}");
        }
    }
}
