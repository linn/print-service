namespace Linn.PrintService.Messaging.Models
{
    public class PrintRsnDocumentMessageBody
    {
        public int RsnNumber { get; set; }

        public string CopyType { get; set; }

        public string FacilityCode { get; set; }

        public string PrinterGroup { get; set; }

        public string? JobName { get; set; }
    }
}
