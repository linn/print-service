namespace Linn.PrintService.Persistence.LinnApps
{
    using Linn.Common.Configuration;
    using Linn.PrintService.Domain.LinnApps;

    using Microsoft.EntityFrameworkCore;

    public class ServiceDbContext : DbContext
    {
        public DbSet<PrinterMapping> PrinterMappings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var host = ConfigurationManager.Configuration["DATABASE_HOST"];
            var userId = ConfigurationManager.Configuration["DATABASE_USER_ID"];
            var password = ConfigurationManager.Configuration["DATABASE_PASSWORD"];
            var name = ConfigurationManager.Configuration["DATABASE_NAME"];

            var connectionString = $"Data Source={host}/{name};User Id={userId};Password={password};";

            optionsBuilder.UseOracle(connectionString, options => options.UseOracleSQLCompatibility("11"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            BuildPrinterMappings(builder);
        }

        private static void BuildPrinterMappings(ModelBuilder builder)
        {
            var e = builder.Entity<PrinterMapping>().ToTable("PRINTER_MAPPINGS");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("ID");
            e.Property(p => p.PrinterName).HasColumnName("PRINTER_NAME").HasMaxLength(100);
            e.Property(p => p.UserNumber).HasColumnName("USER_NUMBER");
            e.Property(p => p.PrinterType).HasColumnName("PRINTER_TYPE").HasMaxLength(50);
            e.Property(p => p.PrinterGroup).HasColumnName("PRINTER_GROUP").HasMaxLength(50);
            e.Property(p => p.DefaultForGroup).HasColumnName("DEFAULT_FOR_GROUP").HasMaxLength(1);
            e.Property(p => p.PrinterUri).HasColumnName("PRINTER_URI").HasMaxLength(500);
            e.Ignore(p => p.IsDefault);
        }
    }
}
