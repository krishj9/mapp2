namespace MAPP.Modules.Planning.Domain.Enums;

/// <summary>
/// Plan status enumeration following Ardalis patterns
/// </summary>
public enum PlanStatus
{
    Draft = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    OnHold = 4
}
