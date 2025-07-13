using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Planning.Domain.Entities;

namespace MAPP.Modules.Planning.Domain.Events;

/// <summary>
/// Domain event raised when a plan is started
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class PlanStartedEvent : BaseEvent
{
    public Plan Plan { get; }

    public PlanStartedEvent(Plan plan)
    {
        Plan = plan;
    }
}
