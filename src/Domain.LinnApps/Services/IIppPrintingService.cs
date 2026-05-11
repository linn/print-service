namespace Linn.PrintService.Domain.LinnApps.Services
{
    using System.Threading.Tasks;

    public interface IIppPrintingService
    {
        Task<PrintResult> Print(string printerUri, string jobName, byte[] data);

        Task<PrintResult> GetDetailedStatus(string printerUri);
    }
}