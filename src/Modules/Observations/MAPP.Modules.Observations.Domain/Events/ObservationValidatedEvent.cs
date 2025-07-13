using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation is validated
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationValidatedEvent : BaseEvent
{
    public Observation Observation { get; }

    public ObservationValidatedEvent(Observation observation)
    {
        Observation = observation;
    }
}
