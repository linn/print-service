namespace Linn.PrintService.Printing.Services
{
    using System.Threading.Tasks;

    public interface IPackingListProxy
    {
        Task<byte[]> GetPackingListAsPdf(int consignmentNumber);
    }
}
