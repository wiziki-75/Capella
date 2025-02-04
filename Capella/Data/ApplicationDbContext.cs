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
        public DbSet<Subscription> Subscriptions { get; set; } // Ajouter la table Subscription

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User table mapping
            modelBuilder.Entity<User>()
                .ToTable("user")
                .HasKey(u => u.Id_User);

            // Post table mapping
            modelBuilder.Entity<Post>()
                .ToTable("cap_post")
                .HasKey(p => p.Id_Post);

            modelBuilder.Entity<Post>()
                .Property(p => p.PostId)
                .HasColumnName("post_id");

            modelBuilder.Entity<Post>()
                .Property(p => p.UserId)
                .HasColumnName("user_id");

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Replies)
                .WithOne()
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Like table mapping
            modelBuilder.Entity<Like>()
                .ToTable("cap_like")
                .HasKey(l => l.Id);

            modelBuilder.Entity<Like>()
                .Property(l => l.Id)
                .HasColumnName("id_post"); // Map la propriété Id à id_post

            modelBuilder.Entity<Like>()
                .Property(l => l.UserId)
                .HasColumnName("user_id");

            modelBuilder.Entity<Like>()
                .Property(l => l.PostId)
                .HasColumnName("post_id");

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany()
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Subscription table mapping
            modelBuilder.Entity<Subscription>()
                .ToTable("cap_subscription") // Nom de la table
                .HasKey(s => s.Id); // Clé primaire

            modelBuilder.Entity<Subscription>()
                .Property(s => s.SubscriberId)
                .HasColumnName("subscriber_id");

            modelBuilder.Entity<Subscription>()
                .Property(s => s.SubscribedToId)
                .HasColumnName("subscribed_to_id");

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Subscriber)
                .WithMany()
                .HasForeignKey(s => s.SubscriberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.SubscribedTo)
                .WithMany()
                .HasForeignKey(s => s.SubscribedToId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
