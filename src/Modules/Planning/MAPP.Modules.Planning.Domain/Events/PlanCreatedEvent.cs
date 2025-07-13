using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Planning.Domain.Entities;

namespace MAPP.Modules.Planning.Domain.Events;

/// <summary>
/// Domain event raised when a plan is created
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class PlanCreatedEvent : BaseEvent
{
    public Plan Plan { get; }

    public PlanCreatedEvent(Plan plan)
    {
        Plan = plan;
    }
}
