using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebApplcationDogFoodEcom.Server.Data;

namespace WebApplcationDogFoodEcom.Server.Repositories;

/// <summary>
/// Generic EF Core implementation of <see cref="IRepository{T}"/>.
/// Derived classes add entity-specific queries.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly PawMedsDbContext Db;
    protected readonly DbSet<T> Set;

    public Repository(PawMedsDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await Set.FindAsync([id], ct);

    public virtual async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default)
        => await Set.AsNoTracking().ToListAsync(ct);

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
        => await Set.AsNoTracking().Where(predicate).ToListAsync(ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        => await Set.AddAsync(entity, ct);

    public virtual void Update(T entity) => Set.Update(entity);

    public virtual void Remove(T entity) => Set.Remove(entity);
}
