using FastEndpoints;
using FastEndpoints.Swagger;
using MAPP.BuildingBlocks.Web;
using MAPP.Modules.Observations.Application;
using MAPP.Modules.Observations.Infrastructure;
using MAPP.Services.Observations.HealthChecks;
using MAPP.ServiceDefaults;
using MAPP.BuildingBlocks.Infrastructure.PubSub;

var builder = WebApplication.CreateBuilder(args);

// 🔍 DEBUG: Log configuration information
Console.WriteLine("🔍 DEBUGGING CONFIGURATION LOADING - BUILD v2:");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Content Root: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
Console.WriteLine($"Build Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

// Log environment variables
Console.WriteLine("🔍 ENVIRONMENT VARIABLES:");
Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
Console.WriteLine($"DATABASE_CONNECTION_STRING: {Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")?.Substring(0, Math.Min(50, Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")?.Length ?? 0))}...");

// Add service defaults & Aspire components
builder.AddServiceDefaults();

// 🔍 DEBUG: Test configuration access
Console.WriteLine("🔍 TESTING CONFIGURATION ACCESS:");
var testConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection string from GetConnectionString: {testConnectionString?.Substring(0, Math.Min(50, testConnectionString?.Length ?? 0))}...");
Console.WriteLine($"Connection string length: {testConnectionString?.Length ?? 0}");

// Test direct configuration access
var directConfig = builder.Configuration["ConnectionStrings:DefaultConnection"];
Console.WriteLine($"Direct config access: {directConfig?.Substring(0, Math.Min(50, directConfig?.Length ?? 0))}...");

// Test environment variable substitution
var envVarValue = builder.Configuration["DATABASE_CONNECTION_STRING"];
Console.WriteLine($"DATABASE_CONNECTION_STRING from config: {envVarValue?.Substring(0, Math.Min(50, envVarValue?.Length ?? 0))}...");

// Add services to the container
builder.Services.AddWebServices();
builder.Services.AddObservationsApplication();
builder.Services.AddObservationsInfrastructure(builder.Configuration);

// Add Pub/Sub services for cross-service communication
builder.Services.AddPubSubForEnvironment(builder.Configuration, builder.Environment);

// Add authentication and authorization (disabled for development)
// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

// Add HTTP client for health checks
builder.Services.AddHttpClient();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<ObservationsHealthCheck>("observations_domain");

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
// Authentication and authorization disabled for development
// app.UseAuthentication();
// app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Endpoints.ShortNames = true;
});

// Add Swagger for development
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

// Map custom health check endpoint
app.MapHealthChecks("/health/detailed", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            domain = "Observations",
            timestamp = DateTimeOffset.UtcNow,
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                data = x.Value.Data
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapDefaultEndpoints();

app.Run();
