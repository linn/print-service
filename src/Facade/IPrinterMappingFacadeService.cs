namespace Linn.PrintService.Facade
{
    using System.Collections.Generic;

    using Linn.Common.Facade;
    using Linn.PrintService.Resources;

    public interface IPrinterMappingFacadeService
    {
        IResult<IEnumerable<PrinterMappingResource>> GetDefaultPrinters();
    }
}
