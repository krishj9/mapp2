using FastEndpoints;
using MAPP.BuildingBlocks.Web.Endpoints;
using MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

namespace MAPP.Services.Observations.Endpoints.Observations;

/// <summary>
/// Create observation endpoint using FastEndpoints following clean architecture patterns
/// </summary>
public class Create : BaseEndpoint<CreateObservationRequest, CreateObservationResponse>
{
    protected override void ConfigureEndpoint()
    {
        Post("/observations");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new observation";
            s.Description = "Creates a new observation with the specified details";
            s.Response<CreateObservationResponse>(201, "Observation created successfully");
            s.Response(400, "Invalid request");
        });
    }

    public override async Task HandleAsync(CreateObservationRequest req, CancellationToken ct)
    {
        var command = new CreateObservationCommand
        {
            ChildId = req.ChildId,
            ChildName = req.ChildName,
            TeacherId = req.TeacherId,
            TeacherName = req.TeacherName,
            DomainId = req.DomainId,
            DomainName = req.DomainName,
            AttributeId = req.AttributeId,
            AttributeName = req.AttributeName,
            ProgressionPointIds = req.ProgressionPointIds,
            ObservationText = req.ObservationText,
            ObservationDate = req.ObservationDate,
            LearningContext = req.LearningContext,
            Tags = req.Tags,
            IsDraft = req.IsDraft
        };

        var observationId = await Mediator.Send(command, ct);

        await SendCreatedAtAsync<GetById>(new { id = observationId }, new CreateObservationResponse { Id = observationId }, cancellation: ct);
    }
}

public class CreateObservationRequest
{
    public long ChildId { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int DomainId { get; set; }
    public string DomainName { get; set; } = string.Empty;
    public int AttributeId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public List<int> ProgressionPointIds { get; set; } = new();
    public string ObservationText { get; set; } = string.Empty;
    public DateTime ObservationDate { get; set; }
    public string? LearningContext { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsDraft { get; set; } = false;
}

public class CreateObservationResponse
{
    public int Id { get; set; }
}
