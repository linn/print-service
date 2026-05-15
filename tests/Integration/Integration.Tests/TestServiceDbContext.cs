namespace Linn.PrintService.Integration.Tests
{
    using System;

    using Linn.PrintService.Domain.LinnApps;

    using Microsoft.EntityFrameworkCore;

    public class TestServiceDbContext : DbContext
    {
        public DbSet<PrinterMapping> PrinterMappings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }
}
