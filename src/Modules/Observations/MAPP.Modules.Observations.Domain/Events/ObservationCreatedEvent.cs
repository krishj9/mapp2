using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation is created
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationCreatedEvent : BaseEvent
{
    public Observation Observation { get; }

    public ObservationCreatedEvent(Observation observation)
    {
        Observation = observation;
    }
}
