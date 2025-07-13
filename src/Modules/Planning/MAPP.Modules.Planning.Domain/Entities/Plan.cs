using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.Planning.Domain.Events;
using MAPP.Modules.Planning.Domain.ValueObjects;
using MAPP.Modules.Planning.Domain.Enums;

namespace MAPP.Modules.Planning.Domain.Entities;

/// <summary>
/// Plan aggregate root following DDD patterns from Ardalis template
/// </summary>
public class Plan : BaseAuditableEntity
{
    private readonly List<PlanItem> _items = new();

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public PlanStatus Status { get; private set; } = PlanStatus.Draft;
    public Priority Priority { get; private set; } = Priority.Medium;
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public string? OwnerId { get; private set; }

    public IReadOnlyCollection<PlanItem> Items => _items.AsReadOnly();

    // Private constructor for EF Core
    private Plan() { }

    public Plan(string title, string? description = null, string? ownerId = null)
    {
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Description = description;
        OwnerId = ownerId;
        Status = PlanStatus.Draft;
        Priority = Priority.Medium;

        AddDomainEvent(new PlanCreatedEvent(this));
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

    public void SetDates(DateTimeOffset? startDate, DateTimeOffset? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be after end date");
        }

        StartDate = startDate;
        EndDate = endDate;
    }

    public void Start()
    {
        if (Status != PlanStatus.Draft)
        {
            throw new InvalidOperationException($"Cannot start plan in {Status} status");
        }

        Status = PlanStatus.InProgress;
        AddDomainEvent(new PlanStartedEvent(this));
    }

    public void Complete()
    {
        if (Status != PlanStatus.InProgress)
        {
            throw new InvalidOperationException($"Cannot complete plan in {Status} status");
        }

        Status = PlanStatus.Completed;
        AddDomainEvent(new PlanCompletedEvent(this));
    }

    public void Cancel()
    {
        if (Status == PlanStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed plan");
        }

        Status = PlanStatus.Cancelled;
        AddDomainEvent(new PlanCancelledEvent(this));
    }

    public PlanItem AddItem(string title, string? description = null)
    {
        var item = new PlanItem(title, description, Id);
        _items.Add(item);
        return item;
    }

    public void RemoveItem(PlanItem item)
    {
        _items.Remove(item);
    }
}
