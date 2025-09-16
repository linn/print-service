namespace Linn.PrintService.Printing.Services
{
    using System.Threading.Tasks;

    public interface IPrintingService
    {
        Task<PrintResult> Print(string printerUri, string jobName, byte[] data);
    }
}