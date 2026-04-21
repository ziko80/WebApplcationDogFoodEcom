using WebApplcationDogFoodEcom.Server.Data;

namespace WebApplcationDogFoodEcom.Server.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PawMedsDbContext _db;

    public UnitOfWork(PawMedsDbContext db, IProductRepository products)
    {
        _db = db;
        Products = products;
    }

    public IProductRepository Products { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
