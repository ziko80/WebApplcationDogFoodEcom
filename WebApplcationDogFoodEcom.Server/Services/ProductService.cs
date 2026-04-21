using WebApplcationDogFoodEcom.Server.Models;
using WebApplcationDogFoodEcom.Server.Repositories;

namespace WebApplcationDogFoodEcom.Server.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public Task<IReadOnlyList<Product>> SearchAsync(
        ProductCategory? category, string? search, CancellationToken ct = default)
        => _repo.SearchAsync(category, search, ct);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repo.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<Product>> GetByCategoryAsync(
        ProductCategory category, CancellationToken ct = default)
        => _repo.GetByCategoryAsync(category, ct);
}
