namespace Linn.PrintService.Messaging.Models
{
    public class PrintPackingListMessageBody
    {
        public string? ConsignmentId { get; set; }

        public string? PrinterUri { get; set; }

        public string? JobName { get; set; }
    }
}
