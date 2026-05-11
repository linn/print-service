namespace Linn.PrintService.Domain.LinnApps.Services
{
    using System.Threading.Tasks;

    public interface IRsnPrintProxy
    {
        Task<byte[]> GetRsnAsPdf(int rsnNumber, string copyType, string facilityCode);
    }
}
