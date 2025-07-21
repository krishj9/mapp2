using MediatR;
using Microsoft.Extensions.Logging;
using MAPP.Modules.Planning.Domain.Events;

namespace MAPP.Modules.Planning.Application.Plans.EventHandlers;

/// <summary>
/// Plan created event handler following clean architecture patterns
/// </summary>
public class PlanCreatedEventHandler : INotificationHandler<PlanCreatedEvent>
{
    private readonly ILogger<PlanCreatedEventHandler> _logger;

    public PlanCreatedEventHandler(ILogger<PlanCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PlanCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MAPP Planning Domain Event: {DomainEvent}", notification.GetType().Name);

        // Add any business logic here, such as:
        // - Sending notifications
        // - Creating audit logs
        // - Triggering workflows
        // - Updating read models

        return Task.CompletedTask;
    }
}
