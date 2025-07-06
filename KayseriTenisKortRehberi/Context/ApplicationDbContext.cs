using Microsoft.EntityFrameworkCore;

namespace KayseriTenisKortRehberi.Context
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Admin> Admins { get; set; }
        public DbSet<Models.Facility> Facilities { get; set; }
        public DbSet<Models.Court> Courts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Models.Admin>().ToTable("Admins");
            modelBuilder.Entity<Models.Facility>().ToTable("Facilities");
            modelBuilder.Entity<Models.Court>().ToTable("Courts");
        }
    }
    
    
}
