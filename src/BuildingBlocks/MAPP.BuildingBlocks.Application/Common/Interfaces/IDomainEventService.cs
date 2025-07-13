using MAPP.BuildingBlocks.Domain.Events;

namespace MAPP.BuildingBlocks.Application.Common.Interfaces;

/// <summary>
/// Service for publishing domain events
/// Adapted from Ardalis Clean Architecture template
/// </summary>
public interface IDomainEventService
{
    Task PublishAsync(BaseEvent domainEvent, CancellationToken cancellationToken = default);
}
