namespace Linn.PrintService.Messaging.Models
{
    public class PrintPackingListMessageBody
    {
        public int ConsignmentId { get; set; }

        public string PrinterUri { get; set; }

        public string? JobName { get; set; }
    }
}
