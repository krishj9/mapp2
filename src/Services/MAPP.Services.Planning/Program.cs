using FastEndpoints;
using FastEndpoints.Swagger;
using MAPP.BuildingBlocks.Web;
using MAPP.Modules.Planning.Application;
using MAPP.Modules.Planning.Infrastructure;
using MAPP.Services.Planning.HealthChecks;
using MAPP.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddWebServices();
builder.Services.AddPlanningApplication();
builder.Services.AddPlanningInfrastructure(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<PlanningHealthCheck>("planning_domain");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseHttpsRedirection();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Endpoints.ShortNames = true;
});

// Map health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            domain = "Planning",
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

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.MapDefaultEndpoints();

app.Run();
