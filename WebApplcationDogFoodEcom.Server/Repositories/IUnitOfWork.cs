namespace WebApplcationDogFoodEcom.Server.Repositories;

/// <summary>
/// Coordinates persistence across repositories that share the same
/// <see cref="Data.PawMedsDbContext"/> instance (per HTTP request).
/// </summary>
public interface IUnitOfWork
{
    IProductRepository Products { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
