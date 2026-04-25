using System.Linq.Expressions;
using WebApplcationDogFoodEcom.Server.Data;
using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Repositories;

/// <summary>
/// In-memory implementation of <see cref="IProductRepository"/> used when
/// no database connection string is configured. Data is seeded from
/// <see cref="ProductStore"/> and mutations (stock decrements) live only
/// for the lifetime of the process.
/// </summary>
public class InMemoryProductRepository : IProductRepository
{
    // Single shared store so stock decrements survive across requests/scopes.
    private static readonly List<Product> _store = InitStore();
    private static readonly Lock _sync = new();

    private static List<Product> InitStore() => ProductStore.Products
        .Select(Clone)
        .ToList();

    private static Product Clone(Product p) => new()
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
    };

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        lock (_sync)
            return Task.FromResult<Product?>(_store.FirstOrDefault(p => p.Id == id) is { } p ? Clone(p) : null);
    }

    public Task<IReadOnlyList<Product>> ListAsync(CancellationToken ct = default)
    {
        lock (_sync)
            return Task.FromResult<IReadOnlyList<Product>>(_store.Select(Clone).ToArray());
    }

    public Task<IReadOnlyList<Product>> FindAsync(
        Expression<Func<Product, bool>> predicate, CancellationToken ct = default)
    {
        var compiled = predicate.Compile();
        lock (_sync)
            return Task.FromResult<IReadOnlyList<Product>>(
                _store.Where(compiled).Select(Clone).ToArray());
    }

    public Task AddAsync(Product entity, CancellationToken ct = default)
    {
        lock (_sync) _store.Add(Clone(entity));
        return Task.CompletedTask;
    }

    public void Update(Product entity)
    {
        lock (_sync)
        {
            var existing = _store.FirstOrDefault(p => p.Id == entity.Id);
            if (existing is null) return;
            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.Price = entity.Price;
            existing.Category = entity.Category;
            existing.Brand = entity.Brand;
            existing.ImageUrl = entity.ImageUrl;
            existing.StockQuantity = entity.StockQuantity;
            existing.DosageInfo = entity.DosageInfo;
            existing.TargetCondition = entity.TargetCondition;
        }
    }

    public void Remove(Product entity)
    {
        lock (_sync) _store.RemoveAll(p => p.Id == entity.Id);
    }

    public Task<IReadOnlyList<Product>> SearchAsync(
        ProductCategory? category, string? search, CancellationToken ct = default)
    {
        lock (_sync)
        {
            IEnumerable<Product> q = _store;
            if (category.HasValue) q = q.Where(p => p.Category == category.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(p =>
                    p.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    p.TargetCondition.Contains(s, StringComparison.OrdinalIgnoreCase));
            }
            return Task.FromResult<IReadOnlyList<Product>>(q.OrderBy(p => p.Id).Select(Clone).ToArray());
        }
    }

    public Task<IReadOnlyList<Product>> GetByCategoryAsync(
        ProductCategory category, CancellationToken ct = default)
    {
        lock (_sync)
            return Task.FromResult<IReadOnlyList<Product>>(
                _store.Where(p => p.Category == category)
                      .OrderBy(p => p.Id)
                      .Select(Clone)
                      .ToArray());
    }

    /// <summary>
    /// Returns the actual tracked instance (not a clone) so that services
    /// can mutate stock. The <see cref="InMemoryUnitOfWork.SaveChangesAsync"/>
    /// call is a no-op because mutations are already visible.
    /// </summary>
    public Task<Product?> GetTrackedAsync(int id, CancellationToken ct = default)
    {
        lock (_sync)
            return Task.FromResult(_store.FirstOrDefault(p => p.Id == id));
    }
}
