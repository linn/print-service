namespace Linn.PrintService.Facade
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Linn.Common.Facade;
    using Linn.PrintService.Resources;

    public interface IPrinterMappingFacadeService
    {
        Task<IResult<IEnumerable<PrinterMappingResource>>> GetDefaultPrinters();
    }
}
