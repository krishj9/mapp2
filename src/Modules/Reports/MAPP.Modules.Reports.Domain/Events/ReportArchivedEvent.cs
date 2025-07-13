using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Reports.Domain.Entities;

namespace MAPP.Modules.Reports.Domain.Events;

public class ReportArchivedEvent : BaseEvent
{
    public Report Report { get; }

    public ReportArchivedEvent(Report report)
    {
        Report = report;
    }
}
