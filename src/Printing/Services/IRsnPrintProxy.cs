namespace Linn.PrintService.Printing.Services
{
    using System.Threading.Tasks;

    public interface IRsnPrintProxy
    {
        Task<byte[]> GetRsnPrintAsPdf(int rsnNumber, string copyType, string facilityCode);
    }
}
