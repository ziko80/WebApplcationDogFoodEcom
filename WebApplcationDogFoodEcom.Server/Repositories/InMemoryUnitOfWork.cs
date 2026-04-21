namespace WebApplcationDogFoodEcom.Server.Repositories;

/// <summary>
/// No-op unit of work used with <see cref="InMemoryProductRepository"/>.
/// Mutations are applied directly to the in-memory store, so
/// <see cref="SaveChangesAsync"/> has nothing to persist.
/// </summary>
public class InMemoryUnitOfWork : IUnitOfWork
{
    public InMemoryUnitOfWork(IProductRepository products) => Products = products;

    public IProductRepository Products { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(0);
}
