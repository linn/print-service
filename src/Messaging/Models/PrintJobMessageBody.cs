namespace Linn.PrintService.Messaging.Models
{
    public class PrintJobMessageBody
    {
        public string? PrinterUri { get; set; }

        public string? JobName { get; set; }

        public byte[]? Data { get; set; }
    }
}
