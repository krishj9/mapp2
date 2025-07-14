using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Represents an observation attribute within a domain for classifying specific child behaviors
/// (e.g., "Uses large muscles to move and balance own body")
/// This is read-only reference data that rarely changes
/// </summary>
public class ObservationAttribute : BaseAuditableEntity
{
    private readonly List<ProgressionPoint> _progressionPoints = new();

    public int Number { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? CategoryInformation { get; private set; }
    public int SortOrder { get; private set; }
    public int DomainId { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual ObservationDomain Domain { get; private set; } = null!;
    public IReadOnlyCollection<ProgressionPoint> ProgressionPoints => _progressionPoints.AsReadOnly();

    // Private constructor for EF Core
    private ObservationAttribute() { }

    public static ObservationAttribute Create(
        int id,
        int number,
        string name,
        int sortOrder,
        int domainId,
        string? categoryInformation = null)
    {
        var attribute = new ObservationAttribute
        {
            Id = id,
            Number = Guard.Against.Negative(number, nameof(number)),
            Name = Guard.Against.NullOrEmpty(name, nameof(name)),
            CategoryInformation = categoryInformation,
            SortOrder = Guard.Against.Negative(sortOrder, nameof(sortOrder)),
            DomainId = Guard.Against.NegativeOrZero(domainId, nameof(domainId)),
            IsActive = true
        };

        return attribute;
    }

    public void AddProgressionPoint(ProgressionPoint progressionPoint)
    {
        Guard.Against.Null(progressionPoint, nameof(progressionPoint));
        
        if (!_progressionPoints.Any(p => p.Id == progressionPoint.Id))
        {
            _progressionPoints.Add(progressionPoint);
        }
    }

    public void AddProgressionPoints(IEnumerable<ProgressionPoint> progressionPoints)
    {
        Guard.Against.Null(progressionPoints, nameof(progressionPoints));
        
        foreach (var progressionPoint in progressionPoints)
        {
            AddProgressionPoint(progressionPoint);
        }
    }

    public void UpdateDetails(int number, string name, int sortOrder, string? categoryInformation = null)
    {
        Number = Guard.Against.Negative(number, nameof(number));
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        SortOrder = Guard.Against.Negative(sortOrder, nameof(sortOrder));
        CategoryInformation = categoryInformation;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
    }
}
