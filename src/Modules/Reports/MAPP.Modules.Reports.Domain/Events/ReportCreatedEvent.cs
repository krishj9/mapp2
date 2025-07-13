using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Reports.Domain.Entities;

namespace MAPP.Modules.Reports.Domain.Events;

/// <summary>
/// Domain event raised when a report is created
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ReportCreatedEvent : BaseEvent
{
    public Report Report { get; }

    public ReportCreatedEvent(Report report)
    {
        Report = report;
    }
}
