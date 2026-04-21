using Microsoft.EntityFrameworkCore;
using WebApplcationDogFoodEcom.Server.Data;
using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(PawMedsDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Product>> SearchAsync(
        ProductCategory? category,
        string? search,
        CancellationToken ct = default)
    {
        var query = Set.AsNoTracking().AsQueryable();

        if (category.HasValue)
            query = query.Where(p => p.Category == category.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(p =>
                EF.Functions.Like(p.Name, $"%{s}%") ||
                EF.Functions.Like(p.Description, $"%{s}%") ||
                EF.Functions.Like(p.TargetCondition, $"%{s}%"));
        }

        return await query.OrderBy(p => p.Id).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(
        ProductCategory category,
        CancellationToken ct = default)
        => await Set.AsNoTracking()
            .Where(p => p.Category == category)
            .OrderBy(p => p.Id)
            .ToListAsync(ct);

    public Task<Product?> GetTrackedAsync(int id, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(p => p.Id == id, ct);
}
