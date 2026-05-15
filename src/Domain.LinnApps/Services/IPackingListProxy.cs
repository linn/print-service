namespace Linn.PrintService.Domain.LinnApps.Services
{
    using System.Threading.Tasks;

    public interface IPackingListProxy
    {
        Task<byte[]> GetPackingListAsPdf(int consignmentNumber);
    }
}
