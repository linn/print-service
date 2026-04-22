namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Exceptions;
    using Linn.PrintService.Messaging.Extensions;
    using Linn.PrintService.Printing.Services;

    public class PrintInvoiceMessageHandler : IMessageHandler
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

        public string RoutingKey { get; } = "print.invoice.document";

        public async Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            this.log.Info("[PrintInvoice] Received a message");

            if (!message.TryGetHeaderAsString("documentNumber", out var documentNumberValue)
                || !message.TryGetHeaderAsString("documentType", out var documentType)
                || !message.TryGetHeaderAsString("printerUri", out var printerUri))
            {
                throw new InvoicePrintMessageException(
                    "Missing required header: documentNumber, documentType, or printerUri");
            }

            if (!int.TryParse(documentNumberValue, out var documentNumber))
            {
                throw new InvoicePrintMessageException(
                    $"Invalid documentNumber header value: '{documentNumberValue}' is not a valid integer");
            }

            message.TryGetHeaderAsBool("showTermsAndConditions", out var showTermsAndConditions);

            var showPrices = !message.TryGetHeaderAsBool("showPrices", out var parsedShowPrices)
                || parsedShowPrices;

            var jobName = message.TryGetHeaderAsString("jobName", out var jobNameValue)
                ? jobNameValue
                : $"Invoice_{documentNumber}";

            this.log.Info(
                $"[PrintInvoice] Fetching PDF for {documentType} {documentNumber}, showTerms={showTermsAndConditions}, showPrices={showPrices}");

            var data = await this.invoicePrintProxy.GetInvoiceAsPdf(
                documentType,
                documentNumber,
                showTermsAndConditions,
                showPrices);

            if (data == null || data.Length == 0)
            {
                throw new InvoicePrintMessageException(
                    $"No PDF data returned for {documentType} {documentNumber}");
            }

            this.log.Info($"[PrintInvoice] Received {data.Length} bytes, printing to {printerUri}");

            await this.printingService.Print(printerUri, jobName, data);

            this.log.Info($"[PrintInvoice] Print job completed: {jobName}");
        }
    }
}
