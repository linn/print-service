namespace Linn.PrintService.Service.Modules
{
    using System.IO;
    using System.Threading.Tasks;
    using Linn.Common.Service;
    using Linn.PrintService.Printing;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Printing.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class PrintModule : IModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("/print-service/print", this.Print);
            app.MapGet("/print-service/detailed-status", this.StatusReport);
            app.MapGet("/print-service/status", this.Status);
        }

        private async Task Print(
            HttpRequest req,
            HttpResponse res,
            string printerUri,
            string jobName,
            IIppPrintingService printingService)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                await req.Body.CopyToAsync(ms);
                data = ms.ToArray();
            }

            PrintResult result;

            try
            {
                result = await printingService.Print(printerUri, jobName, data);
            }
            catch (IppPrintingException e)
            {
                res.StatusCode = StatusCodes.Status400BadRequest;
                await res.WriteAsJsonAsync(new { Error = "Printing Error", Message = e.Message });
                return;
            }

            await res.WriteAsJsonAsync(result);
        }

        private async Task StatusReport(
            HttpRequest req,
            HttpResponse res,
            string printerUri,
            IIppPrintingService printingService)
        {
            PrintResult result;

            try
            {
                result = await printingService.GetDetailedStatus(printerUri);
            }
            catch (IppPrintingException e)
            {
                res.StatusCode = StatusCodes.Status400BadRequest;
                await res.WriteAsJsonAsync(new { Error = "Status Error", Message = e.Message });
                return;
            }

            if (!result.Success)
            {
                res.StatusCode = StatusCodes.Status503ServiceUnavailable;
            }

            await res.WriteAsJsonAsync(result);
        }

        private async Task Status(HttpRequest req, HttpResponse res)
        {
            res.StatusCode = StatusCodes.Status200OK;
            await res.WriteAsJsonAsync(new { Status = "OK" });
        }
    }
}
