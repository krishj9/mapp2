using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation is updated
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationUpdatedEvent : BaseEvent
{
    public Observation Observation { get; }

    public ObservationUpdatedEvent(Observation observation)
    {
        Observation = observation;
    }
} 