# Repository & Unit-of-Work Pattern

Clean data-access layer on top of EF Core, added to
`WebApplcationDogFoodEcom.Server`.

## Why

| Goal | Benefit |
|---|---|
| Decouple endpoints from `DbContext` | Endpoints depend on small, intention-revealing interfaces. |
| Centralize domain queries | `SearchAsync`, `GetByCategoryAsync` etc. live in one place. |
| Testability | Swap `IProductRepository` for an in-memory fake in unit tests — no EF required. |
| Transactional consistency | `IUnitOfWork.SaveChangesAsync` commits mutations across repositories atomically. |

## Structure

```
WebApplcationDogFoodEcom.Server/
└── Repositories/
    ├── IRepository.cs           (generic CRUD contract)
    ├── Repository.cs            (EF Core base implementation)
    ├── IProductRepository.cs    (product-specific queries)
    ├── ProductRepository.cs
    ├── IUnitOfWork.cs           (aggregates repositories + SaveChanges)
    └── UnitOfWork.cs
```

### `IRepository<T>`
Generic operations every aggregate root shares:

```csharp
Task<T?>               GetByIdAsync(int id, CancellationToken ct = default);
Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default);
Task<IReadOnlyList<T>> FindAsync(Expression<Func<T,bool>> predicate, CancellationToken ct = default);
Task                   AddAsync(T entity, CancellationToken ct = default);
void                   Update(T entity);
void                   Remove(T entity);
```

### `Repository<T>`
Thin EF Core implementation — uses `AsNoTracking()` for reads, `DbSet.FindAsync` by key.

### `IProductRepository : IRepository<Product>`
Adds domain-specific queries:

```csharp
Task<IReadOnlyList<Product>> SearchAsync(ProductCategory? category, string? search, CancellationToken ct = default);
Task<IReadOnlyList<Product>> GetByCategoryAsync(ProductCategory category, CancellationToken ct = default);
Task<Product?>               GetTrackedAsync(int id, CancellationToken ct = default); // for stock mutations
```

### `IUnitOfWork`
Single entry point to repositories sharing the same scoped `DbContext`:

```csharp
public interface IUnitOfWork
{
    IProductRepository Products { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

## DI registration (`Program.cs`)

```csharp
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

Both are **scoped** so they share the per-request `PawMedsDbContext`.

## Endpoints now depend on the interface, not EF

### Read path — `ProductEndpoints`
```csharp
group.MapGet("/", async (
    ProductCategory? category, string? search,
    IProductRepository repo, CancellationToken ct) =>
        Results.Ok(await repo.SearchAsync(category, search, ct)));
```

### Write path — `OrderEndpoints` (stock decrement + commit)
```csharp
group.MapPost("/", async (CreateOrderRequest request, IUnitOfWork uow, CancellationToken ct) =>
{
    foreach (var item in request.Items)
    {
        var product = await uow.Products.GetTrackedAsync(item.ProductId, ct);
        // validate, mutate product.StockQuantity ...
    }
    await uow.SaveChangesAsync(ct); // one transaction
});
```

## Testing sketch

```csharp
public class FakeProductRepo : IProductRepository
{
    private readonly List<Product> _items = [...];
    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_items.FirstOrDefault(p => p.Id == id));
    // ... etc.
}

var uow = new FakeUnitOfWork(new FakeProductRepo());
// call endpoint/service with uow, assert result.
```

No EF Core, no test database, no container.

## Adding a new aggregate (e.g. `Order`)

1. Create `IOrderRepository : IRepository<Order>` with any specific queries
   (`GetWithItemsAsync(int id)`).
2. Implement `OrderRepository : Repository<Order>, IOrderRepository`.
3. Add `DbSet<Order>` + configuration to `PawMedsDbContext`.
4. Expose it on `IUnitOfWork`:
   ```csharp
   IOrderRepository Orders { get; }
   ```
5. Register in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IOrderRepository, OrderRepository>();
   ```
6. Inject `IUnitOfWork` in endpoints — commit with `uow.SaveChangesAsync`.

## Notes & trade-offs

- For simple CRUD on a single aggregate, EF Core's `DbContext` *is* already a
  Unit-of-Work + repository. This layer is valuable once you have **multiple
  aggregates** and want domain-focused query methods + test seams.
- Keep repositories **thin** — put business rules (e.g. "don't sell below
  stock zero") in a domain service, not in the repository.
- Prefer `IReadOnlyList<T>` as the return type so callers can't mutate the
  in-memory collection by accident.
