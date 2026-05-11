namespace Linn.PrintService.Persistence.LinnApps
{
    using Linn.Common.Configuration;

    using Microsoft.EntityFrameworkCore;

    public class ServiceDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var host = ConfigurationManager.Configuration["DATABASE_HOST"];
            var userId = ConfigurationManager.Configuration["DATABASE_USER_ID"];
            var password = ConfigurationManager.Configuration["DATABASE_PASSWORD"];
            var name = ConfigurationManager.Configuration["DATABASE_NAME"];

            var connectionString = $"Data Source={host}/{name};User Id={userId};Password={password};";

            optionsBuilder.UseOracle(connectionString, o => o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
