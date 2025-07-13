using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Observation data entity following DDD patterns from Ardalis template
/// </summary>
public class ObservationData : BaseAuditableEntity
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? Unit { get; private set; }
    public int ObservationId { get; private set; }

    // Navigation property
    public Observation Observation { get; private set; } = null!;

    // Private constructor for EF Core
    private ObservationData() { }

    public ObservationData(string key, string value, string? unit, int observationId)
    {
        Key = Guard.Against.NullOrEmpty(key, nameof(key));
        Value = Guard.Against.NullOrEmpty(value, nameof(value));
        Unit = unit;
        ObservationId = observationId;
    }

    public void UpdateValue(string value, string? unit = null)
    {
        Value = Guard.Against.NullOrEmpty(value, nameof(value));
        Unit = unit;
    }
}
