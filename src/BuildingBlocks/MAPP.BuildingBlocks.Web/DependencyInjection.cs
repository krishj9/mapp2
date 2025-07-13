using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Infrastructure.Storage;
using MAPP.BuildingBlocks.Web.Services;

namespace MAPP.BuildingBlocks.Web;

/// <summary>
/// Dependency injection configuration for web building blocks
/// Adapted from Ardalis Clean Architecture template
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                    o.DocumentSettings = s =>
                    {
                        s.Title = "MAPP API";
                        s.Version = "v1";
                        s.Description = "API for the MAPP system including media storage capabilities";
                    };
                });

        return services;
    }

    /// <summary>
    /// Adds web services with media storage support
    /// </summary>
    public static IServiceCollection AddWebServicesWithMediaStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Add base web services
        services.AddWebServices();

        // Add media storage based on environment
        services.AddMediaStorageForEnvironment(configuration, environment);

        return services;
    }
}
