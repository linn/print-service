namespace Linn.PrintService.Messaging.Models
{
    public class PrintInvoiceMessageBody
    {
        public string? DocumentNumber { get; set; }

        public string? DocumentType { get; set; }

        public string? PrinterUri { get; set; }

        public bool ShowTermsAndConditions { get; set; }

        public bool ShowPrices { get; set; } = true;

        public string? JobName { get; set; }
    }
}
