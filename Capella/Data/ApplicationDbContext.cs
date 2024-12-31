using Microsoft.EntityFrameworkCore;
using Capella.Models;

namespace Capella.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Map to your existing tables
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customize mappings if needed to align with existing table structures
            modelBuilder.Entity<User>()
                .ToTable("user") // Map to the existing "users" table in your database
                .HasKey(u => u.Id_User); // Specify the primary key column
        }
    }
}
