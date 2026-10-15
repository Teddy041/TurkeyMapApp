using Microsoft.EntityFrameworkCore;
using TurkeyMapApp.Models.Entities;

namespace TurkeyMapApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<Place> Places => Set<Place>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasIndex(c => c.PlateCode).IsUnique();
            entity.HasIndex(c => c.Code).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(100);
            entity.Property(c => c.Code).HasMaxLength(3);
            entity.Property(c => c.Region).HasMaxLength(100);
            entity.Property(c => c.ImageUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasIndex(p => p.CityId);
            entity.HasIndex(p => p.Category);
            entity.Property(p => p.Name).HasMaxLength(200);
            entity.Property(p => p.Category).HasMaxLength(100);
            entity.Property(p => p.Address).HasMaxLength(500);
            entity.Property(p => p.ImageUrl).HasMaxLength(500);

            entity.HasOne(p => p.City)
                .WithMany(c => c.Places)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
