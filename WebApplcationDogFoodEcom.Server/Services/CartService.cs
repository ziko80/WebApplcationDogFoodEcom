using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Services;

/// <summary>
/// Simple in-process cart (single-user demo). For multi-user production,
/// move state to Redis keyed by user/session id.
/// </summary>
public class CartService : ICartService
{
    private static readonly List<CartItem> _cart = [];
    private static readonly Lock _sync = new();

    private readonly IProductService _products;

    public CartService(IProductService products) => _products = products;

    public CartSnapshot Get()
    {
        lock (_sync) return Snapshot();
    }

    public async Task<ServiceResult<CartSnapshot>> AddAsync(int productId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0)
            return ServiceResult<CartSnapshot>.Fail("Quantity must be greater than zero.");

        var product = await _products.GetByIdAsync(productId, ct);
        if (product is null)
            return ServiceResult<CartSnapshot>.Fail("Product not found.");

        if (product.StockQuantity < quantity)
            return ServiceResult<CartSnapshot>.Fail("Insufficient stock.");

        lock (_sync)
        {
            var existing = _cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing is not null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            return ServiceResult<CartSnapshot>.Ok(Snapshot());
        }
    }

    public CartSnapshot Remove(int productId)
    {
        lock (_sync)
        {
            var item = _cart.FirstOrDefault(c => c.ProductId == productId);
            if (item is not null) _cart.Remove(item);
            return Snapshot();
        }
    }

    public CartSnapshot Clear()
    {
        lock (_sync)
        {
            _cart.Clear();
            return Snapshot();
        }
    }

    private static CartSnapshot Snapshot() => new(
        _cart.ToArray(),
        _cart.Sum(i => i.Total),
        _cart.Sum(i => i.Quantity));
}
