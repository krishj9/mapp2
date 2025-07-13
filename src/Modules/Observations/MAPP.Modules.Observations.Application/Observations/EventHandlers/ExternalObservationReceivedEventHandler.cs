using MediatR;
using Microsoft.Extensions.Logging;
using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

namespace MAPP.Modules.Observations.Application.Observations.EventHandlers;

/// <summary>
/// Handles external observation events received from other systems via Pub/Sub
/// </summary>
public class ExternalObservationReceivedEventHandler : INotificationHandler<ExternalObservationReceivedEvent>
{
    private readonly ILogger<ExternalObservationReceivedEventHandler> _logger;
    private readonly IMediator _mediator;

    public ExternalObservationReceivedEventHandler(
        ILogger<ExternalObservationReceivedEventHandler> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Handle(ExternalObservationReceivedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing external observation from {Source}: {Title}",
            notification.Source, notification.Title);

        try
        {
            // Create a new observation based on the external event
            var createCommand = new CreateObservationCommand
            {
                ChildId = 0, // Placeholder for external observations
                ChildName = "[External System]",
                TeacherId = 0, // Placeholder for external observations  
                TeacherName = "[External System]",
                DomainId = 1, // Default domain for external observations
                DomainName = "External Data",
                AttributeId = 1, // Default attribute for external observations
                AttributeName = "External Observation",
                ObservationText = $"[External] {notification.Title} - Received from {notification.Source}: {notification.Description}",
                ObservationDate = DateTime.UtcNow,
                LearningContext = $"External data from {notification.Source}",
                IsDraft = false,
                Tags = new List<string> { "external", notification.Source.ToLowerInvariant() }
            };

            var observationId = await _mediator.Send(createCommand, cancellationToken);

            _logger.LogInformation(
                "Successfully created observation {ObservationId} from external source {Source}",
                observationId, notification.Source);

            // Optionally, store the mapping between external ID and internal ID
            // This would require a new repository/service for external mappings
            // await _externalMappingService.CreateMappingAsync(
            //     notification.ExternalObservationId, 
            //     result.Id, 
            //     notification.Source,
            //     cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process external observation from {Source}: {Title}",
                notification.Source, notification.Title);
            
            // Depending on your requirements, you might want to:
            // 1. Rethrow to trigger retry mechanisms
            // 2. Store in a dead letter queue
            // 3. Send to an error handling service
            throw;
        }
    }
}

/// <summary>
/// External observation created event from other systems
/// </summary>
public class ExternalObservationCreatedEvent : BaseEvent
{
    public string ObservationId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? ExternalSystemId { get; set; }
}

/// <summary>
/// Internal event raised when an external observation is received
/// </summary>
public class ExternalObservationReceivedEvent : BaseEvent
{
    public string ExternalObservationId { get; }
    public string Title { get; }
    public string? Description { get; }
    public string Source { get; }
    public string? ExternalSystemId { get; }

    public ExternalObservationReceivedEvent(
        string externalObservationId,
        string title,
        string? description,
        string source,
        string? externalSystemId)
    {
        ExternalObservationId = externalObservationId;
        Title = title;
        Description = description;
        Source = source;
        ExternalSystemId = externalSystemId;
    }
}
