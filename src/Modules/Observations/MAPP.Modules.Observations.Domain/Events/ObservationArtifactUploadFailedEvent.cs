using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Domain.Events;

/// <summary>
/// Domain event raised when an observation artifact upload fails
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class ObservationArtifactUploadFailedEvent : BaseEvent
{
    public ObservationArtifact ObservationArtifact { get; }
    public string ErrorMessage { get; }

    public ObservationArtifactUploadFailedEvent(ObservationArtifact observationArtifact, string errorMessage)
    {
        ObservationArtifact = observationArtifact;
        ErrorMessage = errorMessage;
    }
} 