namespace Linn.PrintService.Facade.ResourceBuilders
{
    using System;
    using System.Collections.Generic;

    using Linn.Common.Facade;
    using Linn.PrintService.Domain.LinnApps;
    using Linn.PrintService.Resources;

    public class PrinterMappingResourceBuilder : IBuilder<PrinterMapping>
    {
        public PrinterMappingResource Build(PrinterMapping model, IEnumerable<string> claims)
        {
            return new PrinterMappingResource
            {
                Id = model.Id,
                PrinterName = model.PrinterName,
                PrinterGroup = model.PrinterGroup,
                PrinterUri = model.PrinterUri,
                IsDefault = model.IsDefault
            };
        }

        public string GetLocation(PrinterMapping model)
        {
            throw new NotImplementedException();
        }

        object IBuilder<PrinterMapping>.Build(PrinterMapping entity, IEnumerable<string> claims) =>
            this.Build(entity, claims);
    }
}
