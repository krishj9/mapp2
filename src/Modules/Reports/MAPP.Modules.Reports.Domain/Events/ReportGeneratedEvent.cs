using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Reports.Domain.Entities;

namespace MAPP.Modules.Reports.Domain.Events;

public class ReportGeneratedEvent : BaseEvent
{
    public Report Report { get; }

    public ReportGeneratedEvent(Report report)
    {
        Report = report;
    }
}
