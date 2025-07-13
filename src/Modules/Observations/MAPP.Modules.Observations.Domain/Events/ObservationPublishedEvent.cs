using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation is published (no longer a draft)
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationPublishedEvent : BaseEvent
{
    public Observation Observation { get; }

    public ObservationPublishedEvent(Observation observation)
    {
        Observation = observation;
    }
} 