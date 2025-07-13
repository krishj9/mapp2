using MediatR;
using Microsoft.Extensions.Logging;
using MAPP.Modules.Observations.Domain.Events;

namespace MAPP.Modules.Observations.Application.Observations.EventHandlers;

/// <summary>
/// Observation created event handler following Ardalis patterns
/// </summary>
public class ObservationCreatedEventHandler : INotificationHandler<ObservationCreatedEvent>
{
    private readonly ILogger<ObservationCreatedEventHandler> _logger;

    public ObservationCreatedEventHandler(ILogger<ObservationCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ObservationCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MAPP Observations Domain Event: {DomainEvent}", notification.GetType().Name);

        // Add any business logic here, such as:
        // - Sending notifications
        // - Creating audit logs
        // - Triggering workflows
        // - Updating read models

        return Task.CompletedTask;
    }
}
