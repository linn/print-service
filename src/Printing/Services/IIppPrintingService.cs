namespace Linn.PrintService.Printing.Services
{
    using System.Threading.Tasks;

    public interface IIppPrintingService
    {
        Task<PrintResult> Print(string printerUri, string jobName, byte[] data);

        Task<PrintResult> GetStatus(string printerUri);
    }
}