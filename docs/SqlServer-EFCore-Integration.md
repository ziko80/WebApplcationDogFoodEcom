# SQL Server + EF Core Integration — PawMeds

End-to-end guide for the ORM layer added to `WebApplcationDogFoodEcom.Server`.

> Stack: **.NET 10** • **Entity Framework Core 9** • **SQL Server** (container)
> with **LocalDB fallback** for dev without Docker.

---

## 1. What was added

| File | Purpose |
|---|---|
| `WebApplcationDogFoodEcom.Server/Data/PawMedsDbContext.cs` | EF Core `DbContext` with `DbSet<Product>`, column mapping, indexes. |
| `WebApplcationDogFoodEcom.Server/Data/DbInitializer.cs` | Creates the DB + seeds products from `ProductStore` on startup. |
| `WebApplcationDogFoodEcom.Server/Program.cs` | Registers DbContext, calls initializer, falls back to LocalDB when no Aspire connection string. |
| `WebApplcationDogFoodEcom.Server/Endpoints/ProductEndpoints.cs` | Rewritten to query EF Core instead of the static list. |
| `WebApplcationDogFoodEcom.Server/Endpoints/OrderEndpoints.cs` | Stock decrement now persists to SQL via EF. |
| `WebApplcationDogFoodEcom.AppHost/AppHost.cs` | Optional `AddSqlServer(...)` gated by `USE_SQLSERVER=true`. |

### Packages
**Server** (`WebApplcationDogFoodEcom.Server.csproj`)
- `Aspire.Microsoft.EntityFrameworkCore.SqlServer` (13.2.2)
- bumped `OpenTelemetry.Extensions.Hosting` → 1.15.0 (required by the Aspire integration)

**AppHost** (`WebApplcationDogFoodEcom.AppHost.csproj`)
- `Aspire.Hosting.SqlServer` (13.2.2)

---

## 2. Connection strategy (two modes)

```
┌────────────────────────────────────┬──────────────────────────────────────┐
│ USE_SQLSERVER=true (Docker up)     │ default (no env var set)              │
├────────────────────────────────────┼──────────────────────────────────────┤
│ AppHost starts a SQL Server        │ AppHost skips the container.         │
│ container (`mcr.microsoft.com/     │ Server uses LocalDB:                 │
│ mssql/server`) and supplies the    │   Server=(localdb)\MSSQLLocalDB;     │
│ connection string "pawmedsdb" via  │   Database=PawMeds;                  │
│ Aspire service discovery.          │   Trusted_Connection=True            │
│                                    │                                      │
│ Server uses                        │ Server uses                          │
│ AddSqlServerDbContext("pawmedsdb") │ AddDbContext + UseSqlServer(local)   │
└────────────────────────────────────┴──────────────────────────────────────┘
```

The decision is made at runtime in `Program.cs`:

```csharp
var dbConnection = builder.Configuration.GetConnectionString("pawmedsdb");
if (!string.IsNullOrWhiteSpace(dbConnection))
    builder.AddSqlServerDbContext<PawMedsDbContext>("pawmedsdb");
else
    builder.Services.AddDbContext<PawMedsDbContext>(opts => opts.UseSqlServer(LocalDbConn));
```

---

## 3. DbContext & model

```csharp
public class PawMedsDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(120).IsRequired();
            e.Property(p => p.Price).HasPrecision(10, 2);
            e.Property(p => p.Category).HasConversion<string>().HasMaxLength(16);
            e.HasIndex(p => p.Category);
        });
    }
}
```

Key design decisions:
- `Category` is stored as a **string** (`"Medicine"` / `"Vaccine"`) for readability
  in the DB, while the C# code keeps using the `ProductCategory` enum.
- `Price` uses `decimal(10,2)` — avoids floating-point drift.
- `HasIndex(Category)` speeds up `/api/products/medicines` and `/vaccines`.

---

## 4. Seeding

On startup, `DbInitializer.InitializeAsync` runs once:
1. `Database.EnsureCreatedAsync()` — creates the DB + tables if missing.
2. If `Products` is empty, it copies the 12 rows from `ProductStore` so every
   fresh LocalDB/Docker instance is ready to browse.

> ⚠️ `EnsureCreated` is **fast to set up** but not suitable for production
> schema changes. See §8 for the migration upgrade path.

---

## 5. How to run

### Mode A — LocalDB (no Docker, default)

```powershell
dotnet run --project WebApplcationDogFoodEcom.AppHost
```

- Installs nothing extra — LocalDB ships with Visual Studio 2026.
- First run creates DB `PawMeds` under `(localdb)\MSSQLLocalDB`.
- Inspect with SSMS or Visual Studio's **SQL Server Object Explorer**:
  `(localdb)\MSSQLLocalDB → Databases → PawMeds → Tables → dbo.Products`.

### Mode B — SQL Server container (Docker required)

```powershell
$env:USE_SQLSERVER = "true"
dotnet run --project WebApplcationDogFoodEcom.AppHost
```

- Aspire pulls the `mcr.microsoft.com/mssql/server:2022-latest` image.
- Password + port are auto-generated and injected into the server via
  service discovery — you don't set anything manually.
- Dashboard shows the `sql` container resource + an `pawmedsdb` database.
- `.WithLifetime(ContainerLifetime.Persistent)` keeps the volume across
  restarts so seed data isn't lost.

### Combine flags
```powershell
$env:USE_REDIS="true"; $env:USE_SQLSERVER="true"
dotnet run --project WebApplcationDogFoodEcom.AppHost
```

---

## 6. Endpoint changes

All `/api/products*` routes are now DB-backed:

| Route | Notes |
|---|---|
| `GET /api/products?category=&search=` | `EF.Functions.Like` on name/desc/target-condition. Uses `AsNoTracking()` for read perf. |
| `GET /api/products/{id}` | Single-row lookup. |
| `GET /api/products/medicines` / `/vaccines` | Category-indexed. |

`POST /api/orders` now:
- Loads products **with tracking** so EF can detect the stock mutation.
- `SaveChangesAsync()` persists decrements in one transaction.

---

## 7. Inspecting the data

### LocalDB (Mode A)
Visual Studio → **View › SQL Server Object Explorer** →
`(localdb)\MSSQLLocalDB` → `PawMeds` → `dbo.Products` → *View Data*.

Or via `sqlcmd`:
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d PawMeds -Q "SELECT Id, Name, Category, StockQuantity FROM Products"
```

### Container (Mode B)
Aspire dashboard → `sql` resource → **Connection strings** tab → copy
and connect via SSMS / Azure Data Studio.

---

## 8. Upgrading to EF Core Migrations (recommended for production)

`EnsureCreated` is fine for demos, but for schema evolution use migrations:

```powershell
dotnet tool install --global dotnet-ef
cd WebApplcationDogFoodEcom.Server
dotnet ef migrations add InitialCreate
# in Program.cs, replace EnsureCreatedAsync with:
#   await db.Database.MigrateAsync(ct);
```

Then commit the `Migrations/` folder so deployments apply the same schema.

---

## 9. Roadmap (not yet implemented)

| Step | Benefit |
|---|---|
| Add `DbSet<Order>` + `DbSet<CartItem>` and persist orders | Survives restart. |
| Move `CartEndpoints` from in-memory to DB (or Redis) | Shareable across replicas. |
| Replace `EnsureCreated` with `Migrate()` + checked-in migrations | Safe schema evolution. |
| Wrap order creation in `db.Database.BeginTransactionAsync()` | Full atomicity. |
| Use `DbContextPool` with `AddSqlServerDbContext(...)` | Higher throughput. |
| Put SQL password / conn-string in Azure Key Vault in prod | Secret hygiene. |

---

_Last updated: generated with initial EF Core/SQL Server integration._
