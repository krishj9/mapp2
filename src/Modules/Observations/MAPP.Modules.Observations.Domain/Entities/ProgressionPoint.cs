using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Progression point entity for child observations
/// </summary>
public class ProgressionPoint : BaseAuditableEntity
{
    public int Points { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DomainId { get; private set; }
    public int AttributeId { get; private set; }
    public int? ParentProgressionPointId { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public ProgressionPoint? ParentProgressionPoint { get; private set; }
    public List<ProgressionPoint> ChildProgressionPoints { get; private set; } = new();

    // Private constructor for EF Core
    private ProgressionPoint() { }

    public ProgressionPoint(
        int points,
        string title,
        string description,
        int domainId,
        int attributeId,
        int? parentProgressionPointId = null,
        int displayOrder = 0)
    {
        Points = Guard.Against.Negative(points, nameof(points));
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Description = Guard.Against.NullOrEmpty(description, nameof(description));
        DomainId = Guard.Against.NegativeOrZero(domainId, nameof(domainId));
        AttributeId = Guard.Against.NegativeOrZero(attributeId, nameof(attributeId));
        ParentProgressionPointId = parentProgressionPointId;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void UpdateDetails(string title, string description)
    {
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Description = Guard.Against.NullOrEmpty(description, nameof(description));
    }

    public void UpdatePoints(int points)
    {
        Points = Guard.Against.Negative(points, nameof(points));
    }

    public void SetDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    public void SetParent(int? parentProgressionPointId)
    {
        ParentProgressionPointId = parentProgressionPointId;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsChildOf(int parentId)
    {
        return ParentProgressionPointId == parentId;
    }

    public bool HasParent()
    {
        return ParentProgressionPointId.HasValue;
    }

    public bool HasChildren()
    {
        return ChildProgressionPoints.Count > 0;
    }
} 