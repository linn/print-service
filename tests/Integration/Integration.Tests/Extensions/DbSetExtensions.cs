namespace Linn.PrintService.Integration.Tests.Extensions
{
    using Microsoft.EntityFrameworkCore;

    public static class DbSetExtensions
    {
        public static void AddAndSave<T>(this DbSet<T> set, DbContext context, T entity)
            where T : class
        {
            set.Add(entity);
            context.SaveChanges();
        }
    }
}
