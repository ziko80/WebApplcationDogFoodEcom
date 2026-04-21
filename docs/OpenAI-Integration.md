# OpenAI Integration — PawMeds (Dog Medicine & Vaccine Store)

End-to-end guide for the **AI Pet Health Assistant** feature that was added to
`WebApplcationDogFoodEcom.Server`. This document explains **what was added,
why, and how to configure & extend it**.

> Stack: .NET 10 • ASP.NET Core Minimal APIs • .NET Aspire 13.1 • `Microsoft.Extensions.AI` 10.5.0 • OpenAI `gpt-4o-mini`

---

## 1. Feature Summary

A new endpoint `POST /api/ai/chat` lets customers ask pet-health questions
(e.g. *"My dog keeps scratching, what can help?"*) and receive grounded answers
that recommend **real products from `ProductStore`** using **OpenAI function
calling**.

### Why this approach
| Concern | How it's handled |
|---|---|
| Hallucinated products | LLM is forced to call `SearchProducts` / `ListByCategory` / `GetProductById` tools — it cannot invent SKUs. |
| Safety | System prompt enforces a vet-consultation disclaimer on every reply. |
| Vendor portability | Uses `Microsoft.Extensions.AI` abstractions (`IChatClient`). Swap OpenAI → Azure OpenAI / Ollama with one line. |
| Cost | Default model `gpt-4o-mini` — cheap and fast. |
| Observability | `.UseLogging()` pipeline step logs every tool call. |

---

## 2. Files Added / Modified

| File | Change | Purpose |
|---|---|---|
| `Models/ChatModels.cs` | **new** | Request/response DTOs. |
| `Services/AiAssistantService.cs` | **new** | Wraps `IChatClient`, defines product tools, system prompt. |
| `Endpoints/AiEndpoints.cs` | **new** | Minimal API endpoint `POST /api/ai/chat`. |
| `Program.cs` | modified | Registers `IChatClient` + `AiAssistantService`, maps endpoints. |
| `appsettings.json` | modified | Adds `OpenAI:ApiKey`, `OpenAI:ChatModel`. |
| `WebApplcationDogFoodEcom.Server.csproj` | modified | Added `Microsoft.Extensions.AI`, `Microsoft.Extensions.AI.OpenAI`. |

---

## 3. Step-by-Step Implementation

### Step 1 — Install NuGet packages
Run from the `WebApplcationDogFoodEcom.Server` project folder:

```powershell
dotnet add package Microsoft.Extensions.AI
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
```

These add:
- `Microsoft.Extensions.AI` — vendor-neutral abstractions (`IChatClient`, `AIFunctionFactory`, middleware).
- `Microsoft.Extensions.AI.OpenAI` — adapter that converts the official `OpenAI` SDK client into `IChatClient`.

### Step 2 — Configure the API key
Edit `appsettings.json` (or better, **user-secrets** for dev):

```json
"OpenAI": {
  "ApiKey": "",
  "ChatModel": "gpt-4o-mini"
}
```

For local development do **not** commit the key — use:

```powershell
cd WebApplcationDogFoodEcom.Server
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
```

### Step 3 — Define DTOs (`Models/ChatModels.cs`)
Simple records for request/response and chat history.

### Step 4 — Build the assistant service (`Services/AiAssistantService.cs`)

Key pieces:

1. **System prompt** — enforces role, tone, length, and the **vet disclaimer**.
2. **Tools** — three private methods decorated with `[Description]`, wrapped by
   `AIFunctionFactory.Create(...)`:
   - `SearchProducts(query)`
   - `GetProductById(id)`
   - `ListByCategory(category)`
3. **`ChatOptions`** with low `Temperature = 0.3f` for deterministic catalog
   recommendations.
4. **`ChatAsync`** builds the message list (system → history → new user
   message) and calls `IChatClient.GetResponseAsync`. The middleware
   `UseFunctionInvocation()` handles tool-call round-trips automatically.

### Step 5 — Expose HTTP endpoint (`Endpoints/AiEndpoints.cs`)

```
POST /api/ai/chat
{
  "message": "My dog has fleas, what do you recommend?",
  "history": [
    { "role": "user", "content": "Hi" },
    { "role": "assistant", "content": "Hello! How can I help your pup?" }
  ]
}
```

Response:
```json
{ "reply": "Based on the catalog, consider:\n- NexGard ..." }
```

### Step 6 — Wire up DI in `Program.cs`

```csharp
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
var chatModel    = builder.Configuration["OpenAI:ChatModel"] ?? "gpt-4o-mini";

if (!string.IsNullOrWhiteSpace(openAiApiKey))
{
    builder.Services.AddChatClient(sp =>
        new OpenAIClient(openAiApiKey)
            .GetChatClient(chatModel)
            .AsIChatClient())
        .UseFunctionInvocation()   // automatic tool-call loop
        .UseLogging();              // logs prompts/tool calls

    builder.Services.AddSingleton<AiAssistantService>();
}
```

And later:
```csharp
if (!string.IsNullOrWhiteSpace(openAiApiKey))
    app.MapAiEndpoints();
```

The **guard** means the app still runs fine if no API key is configured — the
AI endpoint simply isn't mapped.

### Step 7 — Build & run
```powershell
dotnet build
dotnet run --project WebApplcationDogFoodEcom.AppHost
```

Open the Aspire dashboard → `server` → test via the Swagger/OpenAPI page at
`/openapi/v1.json` or POST directly to `/api/ai/chat`.

---

## 4. How function calling flows

```
User ─► POST /api/ai/chat
         └─► AiAssistantService.ChatAsync
               └─► IChatClient (OpenAI gpt-4o-mini)
                     │
                     ├─► model decides to call SearchProducts("fleas")
                     │     ◄── JSON result (Product[])
                     ├─► model may call another tool
                     │     ◄── JSON result
                     └─► final natural-language reply
```

`UseFunctionInvocation()` performs the tool-call loop transparently, so your
service code only sees the final reply.

---

## 5. Testing with `curl`

```bash
curl -X POST http://localhost:5000/api/ai/chat ^
  -H "Content-Type: application/json" ^
  -d "{ \"message\": \"My dog has itchy red skin, what do you recommend?\" }"
```

Expected: reply references **Apoquel** (from `ProductStore`) plus a vet
disclaimer.

---

## 6. Swapping providers (optional)

### Azure OpenAI
```csharp
using Azure.AI.OpenAI;
builder.Services.AddChatClient(_ =>
    new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(key))
        .GetChatClient(deployment)
        .AsIChatClient())
    .UseFunctionInvocation();
```
Add package `Aspire.Azure.AI.OpenAI` and wire it from `AppHost.cs`:
```csharp
var openai = builder.AddAzureOpenAI("openai");
server.WithReference(openai);
```

### Local model (Ollama)
```csharp
builder.Services.AddChatClient(new OllamaChatClient(
    new Uri("http://localhost:11434"), "llama3.1"));
```

No other code needs to change.

---

## 7. Roadmap (not yet implemented)

| Priority | Feature | Hook point |
|---|---|---|
| P2 | Semantic product search via embeddings + Redis vector index | `ProductEndpoints.cs` → new `/api/products/ai-search` |
| P3 | Cart upsell suggestions | `CartEndpoints.cs` → new `/api/cart/suggestions` |
| P4 | Personalized invoice email body | `EmailService.SendInvoiceEmailAsync` |
| P5 | AI-enriched product descriptions (admin tool) | offline script over `ProductStore` |

---

## 8. Security & compliance checklist

- [x] API key sourced from configuration, never hard-coded.
- [x] Endpoint degrades gracefully when no key is configured.
- [x] System prompt enforces medical disclaimer.
- [x] LLM cannot return arbitrary products (only tool output).
- [ ] Add rate-limiting (`AddRateLimiter`) before production.
- [ ] Add auth (e.g. cookie/JWT) on `/api/ai/chat` before production.
- [ ] Log & redact PII from chat history if you persist it.

---

_Last updated: generated during initial OpenAI integration scaffold._
