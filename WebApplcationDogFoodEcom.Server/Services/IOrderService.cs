using WebApplcationDogFoodEcom.Server.Models;

namespace WebApplcationDogFoodEcom.Server.Services;

public record OrderItemDto(int ProductId, int Quantity);

public record CreateOrderDto(
    string CustomerName,
    string CustomerEmail,
    string ShippingAddress,
    List<OrderItemDto> Items);

/// <summary>
/// Result of a service operation. Keeps the HTTP layer free of business concerns.
/// </summary>
public record ServiceResult<T>(bool Success, T? Value, string? Error)
{
    public static ServiceResult<T> Ok(T value) => new(true, value, null);
    public static ServiceResult<T> Fail(string error) => new(false, default, error);
}

public interface IOrderService
{
    IReadOnlyList<Order> GetAll();
    Order? GetById(int id);
    Task<ServiceResult<Order>> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
}
