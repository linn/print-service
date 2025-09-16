namespace Linn.PrintService.Service.Modules
{
    using System.Threading.Tasks;

    using Linn.Common.Service.Core;
    using Linn.PrintService.Printing.Services;
    using Linn.PrintService.Resources.RequestResources;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class PrintModule : IModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("print-service/print", this.Print);
        }

        private async Task Print(HttpRequest req, HttpResponse res, IPrintingService printingService, PrintJobRequestResource resource)
        {
            if (resource.Data.Length == 0)
            {
                res.StatusCode = 400;
                await res.WriteAsync("Error: Empty data.");
                return;
            }

            var result = await printingService.Print(resource);

            await res.WriteAsJsonAsync(result);
        }
    }
}