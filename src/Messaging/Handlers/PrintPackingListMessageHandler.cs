namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Domain.LinnApps.Services;
    using Linn.PrintService.Messaging.Exceptions;
    using Linn.PrintService.Messaging.Models;
    using Linn.PrintService.Printing;

    public class PrintPackingListMessageHandler : JsonMessageHandler<PrintPackingListMessageBody>
    {
        private readonly IPackingListProxy packingListProxy;
        private readonly IIppPrintingService printingService;
        private readonly ILog log;

        public PrintPackingListMessageHandler(
            IPackingListProxy packingListProxy,
            IIppPrintingService printingService,
            ILog log)
        {
            this.packingListProxy = packingListProxy;
            this.printingService = printingService;
            this.log = log;
        }

        public override string RoutingKey { get; } = "print.packing-list.document";

        protected override async Task HandleAsync(
            PrintPackingListMessageBody body,
            IReadOnlyDictionary<string, object> headers,
            CancellationToken cancellationToken)
        {
            this.log.Info("[PrintPackingList] Received a message");

            if (body.ConsignmentId is null || body.PrinterUri is null)
            {
                throw new PackingListPrintMessageException(
                    "Missing required field in body: consignmentId or printerUri");
            }

            if (!int.TryParse(body.ConsignmentId, out var consignmentNumber))
            {
                throw new PackingListPrintMessageException(
                    $"Invalid consignmentId value: '{body.ConsignmentId}' is not a valid integer");
            }

            var jobName = body.JobName ?? $"PackingList_{consignmentNumber}";

            this.log.Info($"[PrintPackingList] Fetching PDF for consignment {consignmentNumber}");

            var data = await this.packingListProxy.GetPackingListAsPdf(consignmentNumber);

            if (data == null || data.Length == 0)
            {
                throw new PackingListPrintMessageException(
                    $"No PDF data returned for consignment {consignmentNumber}");
            }

            this.log.Info($"[PrintPackingList] Received {data.Length} bytes, printing to {body.PrinterUri}");

            await this.printingService.Print(body.PrinterUri, jobName, data);

            this.log.Info($"[PrintPackingList] Print job completed: {jobName}");
        }
    }
}
