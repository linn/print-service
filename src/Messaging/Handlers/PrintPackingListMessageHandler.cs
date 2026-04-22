namespace Linn.PrintService.Messaging.Handlers
{
    using Linn.Common.Logging;
    using Linn.Common.Messaging.RabbitMQ;
    using Linn.PrintService.Messaging.Exceptions;
    using Linn.PrintService.Messaging.Extensions;
    using Linn.PrintService.Printing.Services;

    public class PrintPackingListMessageHandler : IMessageHandler
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

        public string RoutingKey { get; } = "print.packing-list.document";

        public async Task HandleAsync(Message message, CancellationToken cancellationToken)
        {
            this.log.Info("[PrintPackingList] Received a message");

            if (!message.TryGetHeaderAsString("consignmentId", out var consignmentNumberValue)
                || !message.TryGetHeaderAsString("printerUri", out var printerUri))
            {
                throw new PackingListPrintMessageException(
                    "Missing required header: consignmentId or printerUri");
            }

            if (!int.TryParse(consignmentNumberValue, out var consignmentNumber))
            {
                throw new PackingListPrintMessageException(
                    $"Invalid consignmentNumber header value: '{consignmentNumberValue}' is not a valid integer");
            }

            var jobName = message.TryGetHeaderAsString("jobName", out var jobNameValue)
                ? jobNameValue
                : $"PackingList_{consignmentNumber}";

            this.log.Info($"[PrintPackingList] Fetching PDF for consignment {consignmentNumber}");

            var data = await this.packingListProxy.GetPackingListAsPdf(consignmentNumber);

            if (data == null || data.Length == 0)
            {
                throw new PackingListPrintMessageException(
                    $"No PDF data returned for consignment {consignmentNumber}");
            }

            this.log.Info($"[PrintPackingList] Received {data.Length} bytes, printing to {printerUri}");

            await this.printingService.Print(printerUri, jobName, data);

            this.log.Info($"[PrintPackingList] Print job completed: {jobName}");
        }
    }
}
