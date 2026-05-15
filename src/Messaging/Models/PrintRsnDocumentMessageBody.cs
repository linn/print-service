namespace Linn.PrintService.Messaging.Models
{
    public class PrintRsnDocumentMessageBody
    {
        public string? RsnNumber { get; set; }

        public string? CopyType { get; set; }

        public string? FacilityCode { get; set; }

        public string? PrinterUri { get; set; }

        public string? JobName { get; set; }
    }
}
