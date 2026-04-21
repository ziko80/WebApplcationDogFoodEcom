using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Services;

public record CartSnapshot(IReadOnlyList<CartItem> Items, decimal TotalAmount, int ItemCount);

public interface ICartService
{
    CartSnapshot Get();
    Task<ServiceResult<CartSnapshot>> AddAsync(int productId, int quantity, CancellationToken ct = default);
    CartSnapshot Remove(int productId);
    CartSnapshot Clear();
}
