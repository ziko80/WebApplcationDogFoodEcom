using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using WebApplcationDogFoodEcom.Server.Data;
using WebApplcationDogFoodEcom.Server.Repositories;
using WebApplcationDogFoodEcom.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Use Redis output cache when a "cache" connection string is configured
// (i.e. the Aspire AppHost started the Redis container). Otherwise fall
// back to an in-memory output cache so the app runs without Docker.
var cacheConnection = builder.Configuration.GetConnectionString("cache");
if (!string.IsNullOrWhiteSpace(cacheConnection))
{
    builder.AddRedisClientBuilder("cache")
        .WithOutputCache();
}
else
{
    builder.Services.AddOutputCache();
}

// ---------------- Data layer (conditional) ----------------
// If a "pawmedsdb" connection string is configured (either via Aspire or
// user secrets/appsettings) we wire up EF Core + SQL Server. Otherwise we
// fall back to an in-memory repository seeded from ProductStore so the
// app runs with zero external dependencies (no Docker, no LocalDB).
var dbConnection = builder.Configuration.GetConnectionString("pawmedsdb");
var useDatabase = !string.IsNullOrWhiteSpace(dbConnection);

// Add services to the container.
builder.Services.AddProblemDetails();

if (useDatabase)
{
    builder.AddSqlServerDbContext<PawMedsDbContext>("pawmedsdb");

    // Scoped — one per HTTP request, matching the DbContext lifetime.
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
}
else
{
    // Singleton — the in-memory store is process-wide shared state.
    builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
    builder.Services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
}

// Domain services (Controller → Service → Repository).
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IBillingService, BillingService>();

// MVC controllers.
builder.Services.AddControllers();

// Email settings from configuration (fallback to empty for PDF-only use)
var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
builder.Services.AddSingleton(emailSettings);
builder.Services.AddSingleton<EmailService>();

// ---------------- OpenAI / AI Assistant ----------------
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
var chatModel = builder.Configuration["OpenAI:ChatModel"] ?? "gpt-4o-mini";

if (!string.IsNullOrWhiteSpace(openAiApiKey))
{
    builder.Services.AddChatClient(sp =>
        new OpenAIClient(openAiApiKey)
            .GetChatClient(chatModel)
            .AsIChatClient())
        .UseFunctionInvocation()
        .UseLogging();

    builder.Services.AddSingleton<AiAssistantService>();
}

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseOutputCache();

// Attribute-routed controllers (Products, Orders, Cart, Billing, Ai).
app.MapControllers();

app.MapDefaultEndpoints();

app.UseFileServer();

app.Run();
