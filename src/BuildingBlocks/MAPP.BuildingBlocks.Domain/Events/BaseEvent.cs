using MediatR;

namespace MAPP.BuildingBlocks.Domain.Events;

/// <summary>
/// Base domain event class adapted from Ardalis Clean Architecture template
/// All domain events should inherit from this class
/// </summary>
public abstract class BaseEvent : INotification
{
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
}
