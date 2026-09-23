namespace Linn.PrintService.Printing
{
    using System.Threading.Tasks;

    public interface IIppPrintingService
    {
        Task<PrintResult> Print(string printerUri, string jobName, byte[] data, bool duplex = false);

        Task<PrintResult> GetDetailedStatus(string printerUri);
    }
}
