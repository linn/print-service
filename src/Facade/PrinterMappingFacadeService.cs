namespace Linn.PrintService.Facade
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Linn.Common.Facade;
    using Linn.Common.Persistence;
    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Resources;

    public class PrinterMappingFacadeService : IPrinterMappingFacadeService
    {
        private readonly IQueryRepository<PrinterMapping> repository;
        private readonly IBuilder<PrinterMapping> resourceBuilder;

        public PrinterMappingFacadeService(
            IQueryRepository<PrinterMapping> repository,
            IBuilder<PrinterMapping> resourceBuilder)
        {
            this.repository = repository;
            this.resourceBuilder = resourceBuilder;
        }

        public async Task<IResult<IEnumerable<PrinterMappingResource>>> GetDefaultPrinters()
        {
            var resources = await Task.Run(() => this.repository
                .FilterBy(p => p.DefaultForGroup == "Y" && p.PrinterType == "A4")
                .Select(p => (PrinterMappingResource)this.resourceBuilder.Build(p, null))
                .ToList());

            return resources.Count == 0
                ? new NotFoundResult<IEnumerable<PrinterMappingResource>>("No default A4 printers configured")
                : new SuccessResult<IEnumerable<PrinterMappingResource>>(resources);
        }
    }
}
