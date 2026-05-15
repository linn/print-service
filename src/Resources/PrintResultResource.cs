namespace Linn.PrintService.Resources
{
    public class PrintResultResource
    {
        public bool Success { get; set; }

        public int HttpStatus { get; set; }

        public string? ResponsePreview { get; set; }

        public string? State { get; set; }
    }
}
