using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Represents an observation domain for classifying child observations (e.g., Physical Development, Social-Emotional Development)
/// This is read-only reference data that rarely changes
/// </summary>
public class ObservationDomain : BaseAuditableEntity
{
    private readonly List<ObservationAttribute> _attributes = new();

    public string Name { get; private set; } = string.Empty;
    public string CategoryName { get; private set; } = string.Empty;
    public string CategoryTitle { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation property
    public IReadOnlyCollection<ObservationAttribute> Attributes => _attributes.AsReadOnly();

    // Private constructor for EF Core
    private ObservationDomain() { }

    public static ObservationDomain Create(
        int id,
        string name,
        string categoryName,
        string categoryTitle,
        int sortOrder)
    {
        var domain = new ObservationDomain
        {
            Id = id,
            Name = Guard.Against.NullOrEmpty(name, nameof(name)),
            CategoryName = Guard.Against.NullOrEmpty(categoryName, nameof(categoryName)),
            CategoryTitle = Guard.Against.NullOrEmpty(categoryTitle, nameof(categoryTitle)),
            SortOrder = Guard.Against.Negative(sortOrder, nameof(sortOrder)),
            IsActive = true
        };

        return domain;
    }

    public void AddAttribute(ObservationAttribute attribute)
    {
        Guard.Against.Null(attribute, nameof(attribute));
        
        if (!_attributes.Any(a => a.Id == attribute.Id))
        {
            _attributes.Add(attribute);
        }
    }

    public void AddAttributes(IEnumerable<ObservationAttribute> attributes)
    {
        Guard.Against.Null(attributes, nameof(attributes));
        
        foreach (var attribute in attributes)
        {
            AddAttribute(attribute);
        }
    }

    public void UpdateDetails(string name, string categoryName, string categoryTitle, int sortOrder)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        CategoryName = Guard.Against.NullOrEmpty(categoryName, nameof(categoryName));
        CategoryTitle = Guard.Against.NullOrEmpty(categoryTitle, nameof(categoryTitle));
        SortOrder = Guard.Against.Negative(sortOrder, nameof(sortOrder));
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
    }
}
