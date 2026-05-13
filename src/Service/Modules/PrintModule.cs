namespace Linn.PrintService.Service.Modules
{
    using System.IO;
    using System.Threading.Tasks;

    using Linn.Common.Service;
    using Linn.Common.Service.Extensions;
    using Linn.PrintService.Facade;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class PrintModule : IModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("/print-service/print", this.Print);
            app.MapGet("/print-service/detailed-status", this.DetailedStatus);
            app.MapGet("/print-service/status", this.Status);
            app.MapGet("/print-service/printer-mappings", this.GetDefaultPrinters);
        }

        private async Task Print(
            HttpRequest req,
            HttpResponse res,
            string printerUri,
            string jobName,
            IPrintFacadeService service)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                await req.Body.CopyToAsync(ms);
                data = ms.ToArray();
            }

            await res.Negotiate(await service.PrintAsync(printerUri, jobName, data));
        }

        private async Task DetailedStatus(
            HttpRequest req,
            HttpResponse res,
            string printerUri,
            IPrintFacadeService service)
        {
            await res.Negotiate(await service.GetDetailedStatusAsync(printerUri));
        }

        private async Task Status(HttpRequest req, HttpResponse res)
        {
            res.StatusCode = StatusCodes.Status200OK;
            await res.WriteAsJsonAsync(new { Status = "OK" });
        }

        private async Task GetDefaultPrinters(
            HttpRequest req,
            HttpResponse res,
            IPrinterMappingFacadeService service)
        {
            await res.Negotiate(service.GetDefaultPrinters());
        }
    }
}
