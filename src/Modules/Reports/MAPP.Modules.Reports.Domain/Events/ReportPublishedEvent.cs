using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Reports.Domain.Entities;

namespace MAPP.Modules.Reports.Domain.Events;

public class ReportPublishedEvent : BaseEvent
{
    public Report Report { get; }

    public ReportPublishedEvent(Report report)
    {
        Report = report;
    }
}
