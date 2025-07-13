using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Infrastructure.Data.Interceptors;
using MAPP.BuildingBlocks.Infrastructure.Services;
using MAPP.Modules.Planning.Application.Common.Interfaces;
using MAPP.Modules.Planning.Infrastructure.Data;

namespace MAPP.Modules.Planning.Infrastructure;

/// <summary>
/// Dependency injection configuration for Planning Infrastructure layer
/// Following Ardalis Clean Architecture patterns
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPlanningInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<PlanningDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IPlanningDbContext>(provider => provider.GetRequiredService<PlanningDbContext>());

        return services;
    }
}
