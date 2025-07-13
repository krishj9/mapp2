using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MAPP.BuildingBlocks.Application.Common.Behaviours;

namespace MAPP.Modules.Observations.Application;

/// <summary>
/// Dependency injection configuration for Observations Application layer
/// Following Ardalis Clean Architecture patterns
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddObservationsApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        return services;
    }
}
