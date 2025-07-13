using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.Observations.Domain.Events;
using MAPP.Modules.Observations.Domain.ValueObjects;
using MAPP.Modules.Observations.Domain.Enums;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Observation aggregate root following DDD patterns from Ardalis template
/// </summary>
public class Observation : BaseAuditableEntity
{
    private readonly List<ObservationData> _data = new();

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ObservationStatus Status { get; private set; } = ObservationStatus.Draft;
    public Priority Priority { get; private set; } = Priority.Medium;
    public DateTimeOffset? ObservedAt { get; private set; }
    public string? ObserverId { get; private set; }
    public string? Location { get; private set; }

    public IReadOnlyCollection<ObservationData> Data => _data.AsReadOnly();

    // Private constructor for EF Core
    private Observation() { }

    public Observation(string title, string? description = null, string? observerId = null)
    {
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Description = description;
        ObserverId = observerId;
        Status = ObservationStatus.Draft;
        Priority = Priority.Medium;
        ObservedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new ObservationCreatedEvent(this));
    }

    public void UpdateDetails(string title, string? description = null)
    {
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Description = description;
    }

    public void SetPriority(Priority priority)
    {
        Priority = priority;
    }

    public void SetLocation(string? location)
    {
        Location = location;
    }

    public void SetObservedAt(DateTimeOffset observedAt)
    {
        ObservedAt = observedAt;
    }

    public void Submit()
    {
        if (Status != ObservationStatus.Draft)
        {
            throw new InvalidOperationException($"Cannot submit observation in {Status} status");
        }

        Status = ObservationStatus.Submitted;
        AddDomainEvent(new ObservationSubmittedEvent(this));
    }

    public void Validate()
    {
        if (Status != ObservationStatus.Submitted)
        {
            throw new InvalidOperationException($"Cannot validate observation in {Status} status");
        }

        Status = ObservationStatus.Validated;
        AddDomainEvent(new ObservationValidatedEvent(this));
    }

    public void Reject(string? reason = null)
    {
        if (Status == ObservationStatus.Validated)
        {
            throw new InvalidOperationException("Cannot reject a validated observation");
        }

        Status = ObservationStatus.Rejected;
        AddDomainEvent(new ObservationRejectedEvent(this, reason));
    }

    public ObservationData AddData(string key, string value, string? unit = null)
    {
        var data = new ObservationData(key, value, unit, Id);
        _data.Add(data);
        return data;
    }

    public void RemoveData(ObservationData data)
    {
        _data.Remove(data);
    }
}
