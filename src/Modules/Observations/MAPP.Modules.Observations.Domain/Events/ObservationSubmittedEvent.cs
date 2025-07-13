using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation is submitted
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationSubmittedEvent : BaseEvent
{
    public Observation Observation { get; }

    public ObservationSubmittedEvent(Observation observation)
    {
        Observation = observation;
    }
}
