using FastEndpoints;
using MediatR;

namespace MAPP.BuildingBlocks.Web.Endpoints;

/// <summary>
/// Base endpoint class for FastEndpoints
/// Adapted from Ardalis Clean Architecture template patterns
/// </summary>
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{
    protected IMediator Mediator => Resolve<IMediator>();

    public override void Configure()
    {
        ConfigureEndpoint();
    }

    protected abstract void ConfigureEndpoint();
}

/// <summary>
/// Base endpoint class for endpoints without request
/// </summary>
public abstract class BaseEndpoint<TResponse> : EndpointWithoutRequest<TResponse>
{
    protected IMediator Mediator => Resolve<IMediator>();

    public override void Configure()
    {
        ConfigureEndpoint();
    }

    protected abstract void ConfigureEndpoint();
}

/// <summary>
/// Base endpoint class for endpoints without response
/// </summary>
public abstract class BaseEndpointWithoutResponse<TRequest> : Endpoint<TRequest>
    where TRequest : notnull
{
    protected IMediator Mediator => Resolve<IMediator>();

    public override void Configure()
    {
        ConfigureEndpoint();
    }

    protected abstract void ConfigureEndpoint();
}
