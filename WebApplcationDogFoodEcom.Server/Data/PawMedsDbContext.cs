using Microsoft.EntityFrameworkCore;
using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Data;

public class PawMedsDbContext : DbContext
{
    public PawMedsDbContext(DbContextOptions<PawMedsDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Brand).HasMaxLength(80);
            entity.Property(p => p.ImageUrl).HasMaxLength(500);
            entity.Property(p => p.DosageInfo).HasMaxLength(200);
            entity.Property(p => p.TargetCondition).HasMaxLength(120);
            entity.Property(p => p.Price).HasPrecision(10, 2);
            entity.Property(p => p.Category).HasConversion<string>().HasMaxLength(16);
            entity.HasIndex(p => p.Category);
        });
    }
}
