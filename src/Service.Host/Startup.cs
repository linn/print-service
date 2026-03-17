namespace Linn.PrintService.Service.Host
{
    using Linn.Common.Logging;
    using Linn.Common.Service;
    using Linn.Common.Service.Extensions;
    using Linn.PrintService.IoC;
    using Linn.PrintService.Printing.Exceptions;
    using Linn.PrintService.Service.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.JsonWebTokens;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddCors();
            services.AddSingleton<IResponseNegotiator, UniversalResponseNegotiator>();

            services.AddCredentialsExtensions();
            services.AddSqsExtensions();
            services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.AddFilter("Microsoft", LogLevel.Warning);
                    builder.AddFilter("System", LogLevel.Warning);
                    builder.AddFilter("Linn", LogLevel.Information);
                });
            services.AddLog();

            services.AddServices();
            services.AddMessaging();
            services.AddHostedService<RabbitChannelInitializer>();

            services.AddAuthorization();

            // we need this line for reflection to work in the modules
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

                    var statusCode = StatusCodes.Status500InternalServerError;

                    if (exception is IppPrintingException)
                    {
                        statusCode = StatusCodes.Status400BadRequest;
                    }

                    context.Response.StatusCode = statusCode;
                    var response = new { error = exception?.Message };
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
