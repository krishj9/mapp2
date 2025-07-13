using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation is rejected
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationRejectedEvent : BaseEvent
{
    public Observation Observation { get; }
    public string? Reason { get; }

    public ObservationRejectedEvent(Observation observation, string? reason = null)
    {
        Observation = observation;
        Reason = reason;
    }
}
