using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Planning.Domain.Entities;

namespace MAPP.Modules.Planning.Domain.Events;

/// <summary>
/// Domain event raised when a plan is completed
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class PlanCompletedEvent : BaseEvent
{
    public Plan Plan { get; }

    public PlanCompletedEvent(Plan plan)
    {
        Plan = plan;
    }
}
