namespace Linn.PrintService.Service.Modules
{
    using System;
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
                await res.WriteAsJsonAsync(new
                                               {
                                                   Error = "Printing Error",
                                                   Message = e.Message
                                               });
                Console.WriteLine(e);
                return;
            }

            await res.WriteAsJsonAsync(result);
        }
    }
}