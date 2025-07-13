using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.Planning.Domain.ValueObjects;
using MAPP.Modules.Planning.Domain.Enums;

namespace MAPP.Modules.Planning.Domain.Entities;

/// <summary>
/// Plan item entity following DDD patterns from Ardalis template
/// </summary>
public class PlanItem : BaseAuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public PlanItemStatus Status { get; private set; } = PlanItemStatus.NotStarted;
    public Priority Priority { get; private set; } = Priority.Medium;
    public DateTimeOffset? DueDate { get; private set; }
    public string? AssignedTo { get; private set; }
    public int PlanId { get; private set; }

    // Navigation property
    public Plan Plan { get; private set; } = null!;

    // Private constructor for EF Core
    private PlanItem() { }

    public PlanItem(string title, string? description, int planId)
    {
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Description = description;
        PlanId = planId;
        Status = PlanItemStatus.NotStarted;
        Priority = Priority.Medium;
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

    public void SetDueDate(DateTimeOffset? dueDate)
    {
        DueDate = dueDate;
    }

    public void AssignTo(string? userId)
    {
        AssignedTo = userId;
    }

    public void Start()
    {
        if (Status != PlanItemStatus.NotStarted)
        {
            throw new InvalidOperationException($"Cannot start item in {Status} status");
        }

        Status = PlanItemStatus.InProgress;
    }

    public void Complete()
    {
        if (Status == PlanItemStatus.Completed)
        {
            throw new InvalidOperationException("Item is already completed");
        }

        Status = PlanItemStatus.Completed;
    }

    public void Block(string? reason = null)
    {
        if (Status == PlanItemStatus.Completed)
        {
            throw new InvalidOperationException("Cannot block a completed item");
        }

        Status = PlanItemStatus.Blocked;
        // Could add a reason property if needed
    }
}
