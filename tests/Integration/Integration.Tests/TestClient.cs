namespace Linn.PrintService.Integration.Tests
{
    using System;
    using System.Net.Http;

    using Linn.Common.Service;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;

    public static class TestClient
    {
        public static HttpClient With<T>(
            Action<IServiceCollection> serviceConfiguration, 
            params Func<RequestDelegate, RequestDelegate>[] _) where T : IModule
        {
            var server = new TestServer(
                new WebHostBuilder()
                    .ConfigureServices(
                        services =>
                        {
                            services.AddRouting();
                            services.Apply(serviceConfiguration);
                            services.AddSingleton<IResponseNegotiator, UniversalResponseNegotiator>();
                        })
                    .Configure(
                        app =>
                            {
                                app.UseRouting();
                                app.UseEndpoints(
                                    builder =>
                                        {
                                            // only map the endpoints for the module under test 
                                            var module
                                                =
                                                (T)Activator.CreateInstance(typeof(T)); // i.e. the module of type T
                                            module.MapEndpoints(builder);

                                            // we have to do the above since our testing context only
                                            // injects the dependencies for the module under test...
                                            // so if we try to MapEndpoints() for all modules like we do in the
                                            // real/non-testing scenario the framework complains since other module's
                                            // dependencies are missing.
                                        });
                            }));

            return server.CreateClient();
        }
    }
}
