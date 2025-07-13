using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Planning.Domain.Entities;

namespace MAPP.Modules.Planning.Domain.Events;

/// <summary>
/// Domain event raised when a plan is cancelled
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class PlanCancelledEvent : BaseEvent
{
    public Plan Plan { get; }

    public PlanCancelledEvent(Plan plan)
    {
        Plan = plan;
    }
}
