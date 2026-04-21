# Frontend Login Gate

A minimal client-side login screen has been added in front of the product
catalog. Until the user signs in they cannot see products; after sign-in the
full product grid is shown.

> This is a **demo-only** auth gate. It is client-side and easily bypassed.
> Before production, replace with real backend auth (JWT / cookie) — see
> "Next steps" below.

## Files

| File | Role |
|---|---|
| `frontend/src/components/Login.tsx` | Username/password form, validates against demo credentials, stores session in `localStorage`. |
| `frontend/src/components/Products.tsx` | Fetches `GET /api/products` and renders category-filtered, searchable grid. |
| `frontend/src/components/pawmeds.css` | Styles for login + product grid. |
| `frontend/src/App.tsx` | Gate: shows `Login` when not authenticated, otherwise `Products`. |

## Demo credentials

```
Username: admin
Password: admin123
```

The `App` reads `localStorage.getItem('pawmeds.auth.user')` on startup so a
browser refresh keeps the user signed in. **Sign out** clears the key.

## Flow

```
App
 ├── not authenticated  ──► <Login onLogin={setAuthUser} />
 └── authenticated      ──► <Products username onLogout />
                                 └── fetch('/api/products')
```

Vite's dev proxy (`frontend/vite.config.ts`) forwards `/api/*` to the .NET
server, so no CORS setup is needed.

## Run

```powershell
dotnet run --project WebApplcationDogFoodEcom.AppHost
```

Open the Aspire dashboard → click `webfrontend` → sign in with the demo
credentials → browse the product grid.

## Next steps (production-grade auth)

1. Add ASP.NET Core Identity / JWT auth in `WebApplcationDogFoodEcom.Server`.
2. Replace `Login.tsx` client-side check with a real `POST /api/auth/login`
   that sets an HttpOnly cookie or returns a JWT.
3. Guard `/api/products`, `/api/cart`, `/api/orders`, `/api/ai/chat` with
   `[Authorize]` / `RequireAuthorization()`.
4. Move the auth state into a React context so nested components can access
   the current user & token.
