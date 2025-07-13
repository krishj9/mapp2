using FastEndpoints;
using FastEndpoints.Swagger;
using MAPP.BuildingBlocks.Web;
using MAPP.Modules.Observations.Application;
using MAPP.Modules.Observations.Infrastructure;
using MAPP.Services.Observations.HealthChecks;
using MAPP.ServiceDefaults;
using MAPP.BuildingBlocks.Infrastructure.PubSub;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components
builder.AddServiceDefaults();

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
