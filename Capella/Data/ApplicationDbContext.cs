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

        // Define DbSet properties for each table
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User table mapping
            modelBuilder.Entity<User>()
                .ToTable("user") // Map to the "user" table in the database
                .HasKey(u => u.Id_User); // Specify the primary key

            // Post table mapping
            modelBuilder.Entity<Post>()
                .ToTable("cap_post") // Map to the "cap_post" table in the database
                .HasKey(p => p.Id_Post); // Specify the primary key

            modelBuilder.Entity<Post>()
                .Property(p => p.PostId)
                .HasColumnName("post_id"); // Map PostId in the model to post_id in the database

            modelBuilder.Entity<Post>()
                .Property(p => p.UserId)
                .HasColumnName("user_id"); // Map UserId in the model to user_id in the database

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User) // Define the relationship between Post and User
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete posts when a user is deleted

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Replies) // Define the self-referencing relationship for replies
                .WithOne()
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete replies when a parent post is deleted

            // Like table mapping
            modelBuilder.Entity<Like>()
                .ToTable("cap_like") // Map to the "cap_like" table
                .HasKey(l => l.Id); // Map Id to id_post (primary key)

            modelBuilder.Entity<Like>()
                .Property(l => l.Id)
                .HasColumnName("id_post"); // Map Id to id_post

            modelBuilder.Entity<Like>()
                .Property(l => l.UserId)
                .HasColumnName("user_id"); // Map UserId to user_id

            modelBuilder.Entity<Like>()
                .Property(l => l.PostId)
                .HasColumnName("post_id"); // Map PostId to post_id

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User) // Define relationship with User
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post) // Define relationship with Post
                .WithMany()
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
