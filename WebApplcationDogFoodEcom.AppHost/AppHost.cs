var builder = DistributedApplication.CreateBuilder(args);

// Redis is optional for local dev — only start the container when Docker
// is available (controlled via launchSettings / env var USE_REDIS=true).
var useRedis = string.Equals(
    builder.Configuration["USE_REDIS"],
    "true",
    StringComparison.OrdinalIgnoreCase);

// SQL Server is optional for local dev — when USE_SQLSERVER is not "true"
// the server falls back to (localdb)\MSSQLLocalDB automatically.
var useSqlServer = string.Equals(
    builder.Configuration["USE_SQLSERVER"],
    "true",
    StringComparison.OrdinalIgnoreCase);

var server = builder.AddProject<Projects.WebApplcationDogFoodEcom_Server>("server")
    .WithExternalHttpEndpoints();

if (useRedis)
{
    var cache = builder.AddRedis("cache");
    server.WithReference(cache).WaitFor(cache);
}

if (useSqlServer)
{
    var sql = builder.AddSqlServer("sql")
        .WithLifetime(ContainerLifetime.Persistent);
    var db = sql.AddDatabase("pawmedsdb");
    server.WithReference(db).WaitFor(db);
}

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
