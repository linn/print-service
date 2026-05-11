namespace Linn.PrintService.IoC
{
    using Linn.Common.Persistence;
    using Linn.Common.Persistence.EntityFramework;
    using Linn.PrintService.Persistence.LinnApps;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            return services.AddScoped<ServiceDbContext>()
                .AddScoped<DbContext>(a => a.GetService<ServiceDbContext>())
                .AddScoped<ITransactionManager, TransactionManager>();
        }
    }
}
