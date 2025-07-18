using ArandaTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArandaTest.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Products> Products { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>(entity =>
            {
                entity.ToTable("Products", "Aranda");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.ShortDescription).HasMaxLength(1000);
                entity.Property(p => p.ImageUrl).HasMaxLength(500);
                entity.Property(p => p.CreatedAt);
                entity.Property(p => p.UpdatedAt);

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId);

                entity.HasOne(ac => ac.Author)
               .WithMany()
               .HasForeignKey(ac => ac.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.CategoryId);
                entity.HasIndex(p => p.CreatedAt);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "Aranda");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(r => r.IsActive);
                entity.Property(p => p.CreatedAt);
                entity.Property(p => p.UpdatedAt);

                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.CreatedAt);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users", "Aranda");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Document).IsRequired().HasMaxLength(20);
                entity.Property(p => p.Email).IsRequired().HasMaxLength(255);
                entity.Property(p => p.Password).IsRequired().HasMaxLength(500);
                entity.Property(r => r.IsActive);
                entity.Property(p => p.CreatedAt);
                entity.Property(p => p.UpdatedAt);

                entity.HasIndex(p => p.Email).IsUnique();
                entity.HasIndex(p => p.Name);
                entity.HasIndex(p => p.CreatedAt);
            });
        }
    }
}
