namespace MAPP.Modules.Observations.Domain.Enums;

/// <summary>
/// Observation status enumeration following Ardalis patterns
/// </summary>
public enum ObservationStatus
{
    Draft = 0,
    Submitted = 1,
    Validated = 2,
    Rejected = 3,
    Archived = 4
}
