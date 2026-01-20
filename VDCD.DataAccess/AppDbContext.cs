using Microsoft.EntityFrameworkCore;
using VDCD.Entities.Custom;

namespace VDCD.DataAccess
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tự động tìm và apply tất cả IEntityTypeConfiguration<T> trong assembly này
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }   
}
