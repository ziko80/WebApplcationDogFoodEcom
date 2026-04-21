# Quickstart — Try the AI Assistant in 3 minutes

## 1. Set your OpenAI key (user-secrets recommended)

```powershell
cd WebApplcationDogFoodEcom.Server
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-REPLACE_ME"
```

## 2. Run the Aspire AppHost

```powershell
dotnet run --project WebApplcationDogFoodEcom.AppHost
```

## 3. Call the endpoint

PowerShell:
```powershell
$body = @{ message = "My dog has fleas, what do you recommend?" } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:<server-port>/api/ai/chat `
                  -Body $body -ContentType application/json
```

You should receive a reply listing products such as **NexGard** or
**Simparica Trio** — each grounded in `ProductStore` — along with a
vet-consultation disclaimer.

See [`OpenAI-Integration.md`](./OpenAI-Integration.md) for full architecture.
