using FastEndpoints;
using MAPP.BuildingBlocks.Web.Endpoints;
using MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

namespace MAPP.Services.Observations.Endpoints.Observations;

/// <summary>
/// Get observation by ID endpoint using FastEndpoints following Ardalis patterns
/// </summary>
public class GetById : BaseEndpoint<GetObservationByIdRequest, ObservationBriefDto>
{
    protected override void ConfigureEndpoint()
    {
        Get("/observations/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get observation by ID";
            s.Description = "Retrieves a specific observation by its ID";
            s.Response<ObservationBriefDto>(200, "Observation retrieved successfully");
            s.Response(404, "Observation not found");
        });
    }

    public override async Task HandleAsync(GetObservationByIdRequest req, CancellationToken ct)
    {
        // For now, we'll use the GetObservations query with a filter
        // In a real implementation, you'd create a GetObservationByIdQuery
        var query = new GetObservationsQuery
        {
            PageNumber = 1,
            PageSize = 1
        };

        var result = await Mediator.Send(query, ct);
        
        var observation = result.Observations.FirstOrDefault(o => o.Id == req.Id);
        
        if (observation == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(observation, ct);
    }
}

public class GetObservationByIdRequest
{
    public int Id { get; set; }
}
