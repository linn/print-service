namespace Linn.PrintService.Printing.Services
{
    using System.Threading.Tasks;

    public interface IInvoicePrintProxy
    {
        Task<byte[]> GetInvoiceAsPdf(string documentType, int documentNumber, bool showTerms, bool showPrices);
    }
}
