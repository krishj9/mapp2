using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation artifact is successfully uploaded
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationArtifactUploadedEvent : BaseEvent
{
    public ObservationArtifact ObservationArtifact { get; }

    public ObservationArtifactUploadedEvent(ObservationArtifact observationArtifact)
    {
        ObservationArtifact = observationArtifact;
    }
} 