namespace Linn.PrintService.Service.Modules
{
    using System.IO;
    using System.Threading.Tasks;

    using Linn.Common.Service.Core;
    using Linn.PrintService.Printing.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class PrintModule : IModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("print-service/print", this.Print);
        }

        private async Task Print(HttpRequest req, HttpResponse res, IPrintingService printingService)
        {
            var jobName = req.Query["jobName"].ToString();
            var printerUri = req.Query["printerUri"].ToString();

            if (string.IsNullOrWhiteSpace(printerUri))
            {
                res.StatusCode = 400;
                await res.WriteAsync("Error: printerUri is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(jobName))
            {
                jobName = "PrintJob";
            }

            byte[] data;
            using (var ms = new MemoryStream())
            {
                await req.Body.CopyToAsync(ms);
                data = ms.ToArray();
            }

            if (data.Length == 0)
            {
                res.StatusCode = 400;
                await res.WriteAsync("Error: Empty data.");
                return;
            }

            var result = await printingService.Print(printerUri, jobName, data);

            await res.WriteAsJsonAsync(result);
        }
    }
}