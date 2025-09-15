namespace Linn.PrintService.Service.Host
{
    using System.IO;

    using Linn.Common.Logging;
    using Linn.Common.Service.Core;
    using Linn.Common.Service.Core.Extensions;
    using Linn.PrintService.Service.Host.Negotiators;
    using Linn.PrintService.Service.Models;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
            
            services.AddCors();
            services.AddSingleton<IViewLoader, ViewLoader>();
            services.AddSingleton<IResponseNegotiator, UniversalResponseNegotiator>();

            ApplicationSettings.Get();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Vary", "Accept");
                await next.Invoke();
            });

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                var log = app.ApplicationServices.GetService<ILog>();
                log.Error(exception?.Message, exception);

                var response = new { error = $"{exception?.Message}  -  {exception?.StackTrace}" };
                await context.Response.WriteAsJsonAsync(response);
            }));

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapEndpoints();
            });
        }
    }
}
