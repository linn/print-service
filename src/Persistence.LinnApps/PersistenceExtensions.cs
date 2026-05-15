namespace Linn.PrintService.Persistence.LinnApps
{
    using Linn.Common.Persistence;
    using Linn.Common.Persistence.EntityFramework;
    using Linn.PrintService.Domain.LinnApps;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            return services.AddScoped<ServiceDbContext>()
                .AddScoped<DbContext>(a => a.GetService<ServiceDbContext>())
                .AddScoped<ITransactionManager, TransactionManager>()
                .AddScoped<IQueryRepository<PrinterMapping>, EntityFrameworkQueryRepository<PrinterMapping>>(
                    r => new EntityFrameworkQueryRepository<PrinterMapping>(r.GetService<ServiceDbContext>()?.PrinterMappings));
        }
    }
}
