using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MAPP.ServiceDefaults;

/// <summary>
/// Service defaults for MAPP applications
/// Simplified version for initial setup
/// </summary>
public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        // Add Serilog
        builder.Services.AddSerilog((services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .Enrich.FromLogContext();
        });

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks to the request pipeline.
        app.MapHealthChecks("/health");

        // All health checks must pass for app to be considered ready to accept traffic after starting
        app.MapHealthChecks("/alive");

        return app;
    }

    private static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

        return builder;
    }
}
