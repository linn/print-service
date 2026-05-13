namespace Linn.PrintService.Resources
{
    public class PrinterMappingResource
    {
        public int Id { get; set; }

        public string PrinterName { get; set; }

        public string PrinterGroup { get; set; }

        public string PrinterUri { get; set; }

        public bool IsDefault { get; set; }
    }
}
