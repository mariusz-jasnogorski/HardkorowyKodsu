using Microsoft.EntityFrameworkCore;

namespace HardkorowyKodsu.Server.Data
{
      public class HardkorowyKodsuDbContext : DbContext
    {
        public HardkorowyKodsuDbContext(DbContextOptions<HardkorowyKodsuDbContext> options)
            : base(options)
        {
        }

        public DbSet<SysColumn> SysColumns { get; set; }

        public DbSet<SysObject> SysObjects { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SysObject>()
                .ToView("objects", schema: "sys"); 

            modelBuilder.Entity<SysColumn>()
                .ToView("COLUMNS", schema: "INFORMATION_SCHEMA");

            base.OnModelCreating(modelBuilder);
        }
    }
}
