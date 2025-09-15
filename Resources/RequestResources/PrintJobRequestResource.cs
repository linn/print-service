namespace Linn.PrintService.Resources.RequestResources
{
    public class PrintJobRequestResource
    {
        public string PrinterUri { get; set; }

        public string JobName { get; set; }

        public byte[]? Data { get; set; }
    }
}
