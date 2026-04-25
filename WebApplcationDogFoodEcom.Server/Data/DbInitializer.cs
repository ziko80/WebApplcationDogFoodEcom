using Microsoft.EntityFrameworkCore;
using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Data;

/// <summary>
/// Ensures the database exists and is seeded with the default PawMeds
/// catalog (copied from the in-memory <see cref="ProductStore"/>).
/// Called once at application startup.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PawMedsDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DbInitializer");

        try
        {
            await db.Database.EnsureCreatedAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to create/connect to PawMeds database. Product endpoints will fail.");
            throw;
        }

        if (!await db.Products.AnyAsync(ct))
        {
            logger.LogInformation("Seeding Products table from ProductStore ({Count} rows)", ProductStore.Products.Count);

            // Clone so EF does not track the static singletons.
            var seed = ProductStore.Products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category,
                PetType = p.PetType,
                Brand = p.Brand,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                DosageInfo = p.DosageInfo,
                TargetCondition = p.TargetCondition
            });

            db.Products.AddRange(seed);
            await db.SaveChangesAsync(ct);
        }
        else
        {
            // Add any new products from ProductStore that are missing in the DB.
            var existingIds = await db.Products.Select(p => p.Id).ToListAsync(ct);
            var missing = ProductStore.Products
                .Where(p => !existingIds.Contains(p.Id))
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Category = p.Category,
                    PetType = p.PetType,
                    Brand = p.Brand,
                    ImageUrl = p.ImageUrl,
                    StockQuantity = p.StockQuantity,
                    DosageInfo = p.DosageInfo,
                    TargetCondition = p.TargetCondition
                })
                .ToList();

            if (missing.Count > 0)
            {
                logger.LogInformation("Adding {Count} new products from ProductStore", missing.Count);
                db.Products.AddRange(missing);

                // Also update existing products that may have stale category/PetType values.
                var existingProducts = await db.Products.ToListAsync(ct);
                foreach (var existing in existingProducts)
                {
                    var source = ProductStore.Products.FirstOrDefault(p => p.Id == existing.Id);
                    if (source is not null)
                    {
                        existing.Category = source.Category;
                        existing.PetType = source.PetType;
                    }
                }

                await db.SaveChangesAsync(ct);
            }
        }
    }
}
