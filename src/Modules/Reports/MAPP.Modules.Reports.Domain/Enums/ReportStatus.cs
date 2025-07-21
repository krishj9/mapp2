namespace MAPP.Modules.Reports.Domain.Enums;

/// <summary>
/// Report status enumeration following clean architecture patterns
/// </summary>
public enum ReportStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
    Deprecated = 3
}
