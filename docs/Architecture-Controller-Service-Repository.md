# Architecture — Controller / Service / Repository

The server is organized into four well-isolated layers on top of EF Core
and SQL Server.

```
            HTTP                     Domain                   Data
 ┌──────────────────────┐    ┌──────────────────────┐    ┌─────────────────────┐
 │    Controllers       │───►│       Services       │───►│    Repositories     │───► DbContext ──► SQL
 │ (Products, Orders,   │    │ (ProductService,     │    │ (ProductRepository, │
 │  Cart, Billing, Ai)  │    │  OrderService, …)    │    │  UnitOfWork)        │
 └──────────────────────┘    └──────────────────────┘    └─────────────────────┘
    ▲      only HTTP            ▲  only business rules     ▲  only persistence
    │      concerns             │  & validation            │  concerns
```

## 1. Controllers (`Controllers/*.cs`)

- Inherit `ControllerBase`.
- `[ApiController]` + `[Route("api/[controller]")]` so the class name drives the URL.
- **Only** translate HTTP ↔ service calls. No EF, no business rules.
- Map `ServiceResult<T>` to appropriate HTTP status codes:

```csharp
[HttpPost]
public async Task<ActionResult<Order>> Create(CreateOrderDto dto, CancellationToken ct)
{
    var r = await _service.CreateAsync(dto, ct);
    return r.Success ? CreatedAtAction(nameof(GetById), new { id = r.Value!.Id }, r.Value)
                     : BadRequest(r.Error);
}
```

## 2. Services (`Services/*.cs`)

- Each controller depends on **one** service interface.
- Services depend on repositories, other services, or the unit-of-work.
- Return domain objects or `ServiceResult<T>` for operations that may fail
  with a business-level error.

```csharp
public async Task<ServiceResult<Order>> CreateAsync(CreateOrderDto dto, CancellationToken ct)
{
    if (dto.Items.Count == 0)
        return ServiceResult<Order>.Fail("Order must contain at least one item");
    ...
}
```

### The `ServiceResult<T>` record

```csharp
public record ServiceResult<T>(bool Success, T? Value, string? Error)
{
    public static ServiceResult<T> Ok(T v)     => new(true, v, null);
    public static ServiceResult<T> Fail(string e) => new(false, default, e);
}
```

- No exceptions for expected validation failures.
- Keeps controllers free of try/catch for business rules.
- Easy to translate to Problem Details in a single place if needed.

## 3. Repositories (`Repositories/*.cs`)

See `docs/Repository-Pattern.md` for full details. Summary:

- `IRepository<T>` — generic CRUD (`GetByIdAsync`, `ListAsync`, `FindAsync`, `AddAsync`, `Update`, `Remove`).
- `IProductRepository : IRepository<Product>` — adds `SearchAsync`, `GetByCategoryAsync`, `GetTrackedAsync`.
- `IUnitOfWork` — exposes `Products` + `SaveChangesAsync` so multiple operations commit in one transaction.

## 4. Models (`Models/*.cs`)

- EF-mapped entities (`Product`, `Order`, `CartItem`) with no persistence
  attributes — configuration lives in `PawMedsDbContext.OnModelCreating`.
- Keep them plain. DTOs specific to a service live next to the service
  (`CreateOrderDto` in `IOrderService.cs`).

## 5. Request flow example — `POST /api/orders`

```
HTTP POST /api/orders                          OrdersController.Create
          CreateOrderDto ───────────────────►  IOrderService.CreateAsync
                                               ├─ foreach item:
                                               │     IUnitOfWork.Products.GetTrackedAsync
                                               │     mutate product.StockQuantity
                                               └─ IUnitOfWork.SaveChangesAsync  ──► SQL COMMIT
                                               return ServiceResult<Order>.Ok(order)
          HTTP 201 Created ◄──────────────────  map to CreatedAtAction(...)
```

## 6. DI lifetimes

| Service | Lifetime | Why |
|---|---|---|
| `PawMedsDbContext` | Scoped (EF default via `AddDbContext`) | One per HTTP request. |
| `IProductRepository` / `IUnitOfWork` | Scoped | Share the same `DbContext`. |
| `IProductService`, `IOrderService`, `ICartService`, `IBillingService` | Scoped | Depend on scoped repos/services. |
| `EmailService` | Singleton | Stateless + holds SMTP settings. |
| `AiAssistantService` | Singleton | Wraps an `IChatClient`. |
| Controllers | Transient (default) | Created per request by MVC. |

## 7. Testing sketch

Unit-test a service without EF or HTTP:

```csharp
public class FakeProductRepo : IProductRepository { ... }
public class FakeUnitOfWork : IUnitOfWork {
    public IProductRepository Products { get; init; } = default!;
    public int Saved { get; private set; }
    public Task<int> SaveChangesAsync(CancellationToken ct = default) { Saved++; return Task.FromResult(1); }
}

[Fact]
public async Task Create_ReturnsFail_WhenStockInsufficient()
{
    var repo = new FakeProductRepo(/* product with StockQuantity=1 */);
    var uow  = new FakeUnitOfWork { Products = repo };
    var svc  = new OrderService(uow);

    var dto  = new CreateOrderDto("a","a@b","addr",[new(1, 999)]);
    var r    = await svc.CreateAsync(dto);

    Assert.False(r.Success);
    Assert.Contains("Insufficient stock", r.Error);
}
```

Integration-test a controller with `WebApplicationFactory<Program>` —
swap the DbContext to an in-memory or SQLite provider in the test fixture.

## 8. Adding a new feature — recipe

1. Define/extend the entity + DbContext configuration.
2. Add repository interface/implementation (or reuse the generic base).
3. Add service interface with the business operations you need.
4. Implement the service, returning `ServiceResult<T>` on failure paths.
5. Create the controller with attribute routes; translate results to
   `Ok / NotFound / BadRequest / Created`.
6. Register repo + service in `Program.cs` as `Scoped`.
7. Write unit tests against fakes + a couple of integration tests.

## 9. What's still in-memory (roadmap)

- `OrderService` keeps a static `List<Order>` — persist to a `DbSet<Order>`
  when you're ready. The rest of the architecture will not change.
- `CartService` uses a static cart — move state to Redis keyed by user
  id/session to support multi-user / multi-instance deployments.
