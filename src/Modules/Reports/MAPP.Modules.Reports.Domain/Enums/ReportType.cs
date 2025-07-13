namespace MAPP.Modules.Reports.Domain.Enums;

/// <summary>
/// Report type enumeration following Ardalis patterns
/// </summary>
public enum ReportType
{
    Standard = 0,
    Dashboard = 1,
    Scheduled = 2,
    AdHoc = 3,
    Export = 4
}
