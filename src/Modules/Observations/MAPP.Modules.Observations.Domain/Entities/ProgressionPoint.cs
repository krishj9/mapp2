using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Represents a progression point within an attribute for rating child development levels
/// (e.g., "Emerging", "Developing", "Progressing", "Advancing", "Refining")
/// This is read-only reference data that rarely changes
/// </summary>
public class ProgressionPoint : BaseAuditableEntity
{
    public int Points { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Order { get; private set; }
    public string? CategoryInformation { get; private set; }
    public int SortOrder { get; private set; }
    public int AttributeId { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation property
    public virtual ObservationAttribute Attribute { get; private set; } = null!;

    // Private constructor for EF Core
    private ProgressionPoint() { }

    public static ProgressionPoint Create(
        int progressionId,
        int points,
        string title,
        string description,
        int sortOrder,
        int attributeId,
        string? order = null,
        string? categoryInformation = null)
    {
        var progressionPoint = new ProgressionPoint
        {
            Id = progressionId,
            Points = Guard.Against.Negative(points, nameof(points)),
            Title = Guard.Against.NullOrEmpty(title, nameof(title)),
            Description = Guard.Against.NullOrEmpty(description, nameof(description)),
            Order = order,
            CategoryInformation = categoryInformation,
            SortOrder = Guard.Against.Negative(sortOrder, nameof(sortOrder)),
            AttributeId = Guard.Against.NegativeOrZero(attributeId, nameof(attributeId)),
            IsActive = true
        };

        return progressionPoint;
    }

    public void UpdateDetails(
        int points,
        string title,
        string description,
        int sortOrder,
        string? order = null,
        string? categoryInformation = null)
    {
        Points = Guard.Against.Negative(points, nameof(points));
        Title = Guard.Against.NullOrEmpty(title, nameof(title));
        Description = Guard.Against.NullOrEmpty(description, nameof(description));
        SortOrder = Guard.Against.Negative(sortOrder, nameof(sortOrder));
        Order = order;
        CategoryInformation = categoryInformation;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
    }
} 