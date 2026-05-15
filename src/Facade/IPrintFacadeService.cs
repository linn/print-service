namespace Linn.PrintService.Facade
{
    using System.Threading.Tasks;

    using Linn.Common.Facade;
    using Linn.PrintService.Resources;

    public interface IPrintFacadeService
    {
        Task<IResult<PrintResultResource>> PrintAsync(string printerUri, string jobName, byte[] data);

        Task<IResult<PrintResultResource>> GetDetailedStatusAsync(string printerUri);
    }
}
