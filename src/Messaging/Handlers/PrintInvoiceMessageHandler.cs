namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Domain.LinnApps.Services;
    using Linn.PrintService.Messaging.Exceptions;
    using Linn.PrintService.Messaging.Models;
    using Linn.PrintService.Printing;

    public class PrintInvoiceMessageHandler : JsonMessageHandler<PrintInvoiceMessageBody>
    {
        private readonly IInvoicePrintProxy invoicePrintProxy;
        private readonly IIppPrintingService printingService;
        private readonly ILog log;

        public PrintInvoiceMessageHandler(
            IInvoicePrintProxy invoicePrintProxy,
            IIppPrintingService printingService,
            ILog log)
        {
            this.invoicePrintProxy = invoicePrintProxy;
            this.printingService = printingService;
            this.log = log;
        }

        public override string RoutingKey { get; } = "print.invoice.document";

        public override async Task HandleAsync(
            PrintInvoiceMessageBody body,
            IReadOnlyDictionary<string, object> headers,
            CancellationToken cancellationToken)
        {
            this.log.Info("[PrintInvoice] Received a message");

            if (body.DocumentNumber == 0 || body.DocumentType is null || body.PrinterUri is null)
            {
                throw new InvoicePrintMessageException(
                    "Missing required field in body: documentNumber, documentType, or printerUri");
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

            this.log.Info($"[PrintInvoice] Received {data.Length} bytes, printing to {body.PrinterUri}");

            await this.printingService.Print(body.PrinterUri, jobName, data);

            this.log.Info($"[PrintInvoice] Print job completed: {jobName}");
        }
    }
}
