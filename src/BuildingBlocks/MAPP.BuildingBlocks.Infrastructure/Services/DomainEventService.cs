using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Domain.Events;
using MAPP.BuildingBlocks.Infrastructure.PubSub;

namespace MAPP.BuildingBlocks.Infrastructure.Services;

/// <summary>
/// Enhanced domain event service implementation
/// Supports both local MediatR and external Pub/Sub publishing
/// Adapted from Ardalis Clean Architecture template
/// </summary>
public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _mediator;
    private readonly IPubSubService? _pubSubService;
    private readonly PubSubOptions _pubSubOptions;

    public DomainEventService(
        ILogger<DomainEventService> logger,
        IPublisher mediator,
        IPubSubService? pubSubService = null,
        IOptions<PubSubOptions>? pubSubOptions = null)
    {
        _logger = logger;
        _mediator = mediator;
        _pubSubService = pubSubService;
        _pubSubOptions = pubSubOptions?.Value ?? new PubSubOptions();
    }

    public async Task PublishAsync(BaseEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);

        // Always publish locally via MediatR for immediate processing
        await _mediator.Publish(domainEvent, cancellationToken);

        // Optionally publish to external Pub/Sub for cross-service communication
        if (_pubSubService != null && _pubSubOptions.EnablePublishing)
        {
            try
            {
                await _pubSubService.PublishAsync(domainEvent, cancellationToken);
                _logger.LogInformation("Successfully published domain event {EventType} to Pub/Sub",
                    domainEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish domain event {EventType} to Pub/Sub",
                    domainEvent.GetType().Name);
                // Don't throw - local processing should continue even if external publishing fails
            }
        }
    }
}
