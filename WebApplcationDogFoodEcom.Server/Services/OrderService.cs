using WebApplcationDogFoodEcom.Server.Models;
using WebApplcationDogFoodEcom.Server.Repositories;

namespace WebApplcationDogFoodEcom.Server.Services;

/// <summary>
/// In-memory order log (placeholder until Orders are persisted to the DB).
/// Uses <see cref="IUnitOfWork"/> to decrement product stock atomically.
/// </summary>
public class OrderService : IOrderService
{
    private static readonly List<Order> _orders = [];
    private static int _nextId = 1;
    private static readonly Lock _sync = new();

    private readonly IUnitOfWork _uow;

    public OrderService(IUnitOfWork uow) => _uow = uow;

    public IReadOnlyList<Order> GetAll()
    {
        lock (_sync) return _orders.ToArray();
    }

    public Order? GetById(int id)
    {
        lock (_sync) return _orders.FirstOrDefault(o => o.Id == id);
    }

    public async Task<ServiceResult<Order>> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return ServiceResult<Order>.Fail("Order must contain at least one item");

        var orderItems = new List<CartItem>();

        foreach (var item in dto.Items)
        {
            var product = await _uow.Products.GetTrackedAsync(item.ProductId, ct);
            if (product is null)
                return ServiceResult<Order>.Fail($"Product {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                return ServiceResult<Order>.Fail($"Insufficient stock for {product.Name}");

            product.StockQuantity -= item.Quantity;
            orderItems.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = item.Quantity
            });
        }

        await _uow.SaveChangesAsync(ct);

        Order order;
        lock (_sync)
        {
            order = new Order
            {
                Id = _nextId++,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                ShippingAddress = dto.ShippingAddress,
                Items = orderItems
            };
            _orders.Add(order);
        }

        return ServiceResult<Order>.Ok(order);
    }
}
