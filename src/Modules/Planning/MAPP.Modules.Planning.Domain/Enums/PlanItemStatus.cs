namespace MAPP.Modules.Planning.Domain.Enums;

/// <summary>
/// Plan item status enumeration following Ardalis patterns
/// </summary>
public enum PlanItemStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Blocked = 3,
    Cancelled = 4
}
