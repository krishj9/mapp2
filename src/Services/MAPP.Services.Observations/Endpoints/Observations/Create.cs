using FastEndpoints;
using MAPP.BuildingBlocks.Web.Endpoints;
using MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

namespace MAPP.Services.Observations.Endpoints.Observations;

/// <summary>
/// Create observation endpoint using FastEndpoints following Ardalis patterns
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
            Title = req.Title,
            Description = req.Description,
            Priority = req.Priority,
            ObservedAt = req.ObservedAt,
            Location = req.Location
        };

        var observationId = await Mediator.Send(command, ct);

        await SendCreatedAtAsync<GetById>(new { id = observationId }, new CreateObservationResponse { Id = observationId }, cancellation: ct);
    }
}

public class CreateObservationRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; } = 2; // Medium priority
    public DateTimeOffset? ObservedAt { get; set; }
    public string? Location { get; set; }
}

public class CreateObservationResponse
{
    public int Id { get; set; }
}
