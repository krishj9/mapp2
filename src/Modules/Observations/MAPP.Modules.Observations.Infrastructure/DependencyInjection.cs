using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Infrastructure.Data.Interceptors;
using MAPP.BuildingBlocks.Infrastructure.Services;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Infrastructure.Data;

namespace MAPP.Modules.Observations.Infrastructure;

/// <summary>
/// Dependency injection configuration for Observations Infrastructure layer
/// Following Ardalis Clean Architecture patterns
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddObservationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Create a temporary logger for debugging
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger("ObservationsInfrastructure");

        // Try to get connection string from configuration first, then fallback to environment variable
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // If connection string is empty or null, try to get it from environment variable
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        }

        // Log detailed connection string information for debugging
        logger.LogInformation("🔍 DEBUGGING CONNECTION STRING:");
        logger.LogInformation("Environment: {Environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
        logger.LogInformation("Connection string from config: {ConnectionString}", configuration.GetConnectionString("DefaultConnection") ?? "NULL");
        logger.LogInformation("Connection string from config length: {Length}", configuration.GetConnectionString("DefaultConnection")?.Length ?? 0);

        // Log all connection string related environment variables
        var dbConnEnvVar = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        logger.LogInformation("DATABASE_CONNECTION_STRING env var: {EnvVar}", dbConnEnvVar ?? "NULL");
        logger.LogInformation("DATABASE_CONNECTION_STRING length: {Length}", dbConnEnvVar?.Length ?? 0);
        logger.LogInformation("Final connection string to use: {FinalConnectionString}", connectionString ?? "NULL");
        logger.LogInformation("Final connection string length: {FinalLength}", connectionString?.Length ?? 0);

        // Log configuration sources for debugging
        if (configuration is IConfigurationRoot configRoot)
        {
            logger.LogInformation("Configuration providers:");
            foreach (var provider in configRoot.Providers)
            {
                logger.LogInformation("- {ProviderType}", provider.GetType().Name);
            }
        }

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        // Log the final connection string that will be used (mask password)
        var maskedConnectionString = connectionString.Contains("Password=")
            ? System.Text.RegularExpressions.Regex.Replace(connectionString, @"Password=[^;]*", "Password=***")
            : connectionString;
        logger.LogInformation("Using masked connection string: {MaskedConnectionString}", maskedConnectionString);

        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ObservationsDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IObservationsDbContext>(provider => provider.GetRequiredService<ObservationsDbContext>());

        return services;
    }
}
