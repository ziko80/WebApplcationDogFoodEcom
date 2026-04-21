using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> SearchAsync(
        ProductCategory? category,
        string? search,
        CancellationToken ct = default);

    Task<IReadOnlyList<Product>> GetByCategoryAsync(
        ProductCategory category,
        CancellationToken ct = default);

    /// <summary>
    /// Loads a product with change tracking so callers can mutate stock
    /// and commit via <see cref="IUnitOfWork.SaveChangesAsync"/>.
    /// </summary>
    Task<Product?> GetTrackedAsync(int id, CancellationToken ct = default);
}
