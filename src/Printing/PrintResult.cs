namespace Linn.PrintService.Printing
{
    public class PrintResult
    {
        public bool Success { get; set; }

        public int HttpStatus { get; set; }

        public string? ResponsePreview { get; set; }
    }
}
