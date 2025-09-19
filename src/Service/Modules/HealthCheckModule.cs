namespace Linn.PrintService.Service.Modules
{
    using Linn.Common.Service;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class HealthCheckModule : IModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/healthcheck", async (HttpRequest req, HttpResponse res) => await res.WriteAsync("Ok"));
        }
    }
}
