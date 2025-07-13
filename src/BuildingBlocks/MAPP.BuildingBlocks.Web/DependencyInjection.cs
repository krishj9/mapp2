using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.DependencyInjection;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
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
                        s.Title = "MAPP Observations API";
                        s.Version = "v1";
                        s.Description = "API for managing observations in the MAPP system";
                    };
                });

        return services;
    }
}
