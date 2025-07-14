using FastEndpoints;
using MediatR;
using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;

namespace MAPP.Services.Observations.Features.Classifications;

/// <summary>
/// Request model for getting classification data
/// </summary>
public class GetClassificationDataRequest
{
    public bool IncludeInactive { get; set; } = false;
}

/// <summary>
/// Fast Endpoint for getting complete classification data
/// </summary>
public class GetClassificationDataEndpoint : Endpoint<GetClassificationDataRequest, ClassificationDataVm>
{
    private readonly IMediator _mediator;

    public GetClassificationDataEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/observations/classifications");
        AllowAnonymous(); // TODO: Add proper authorization
        Summary(s =>
        {
            s.Summary = "Get complete classification data";
            s.Description = "Returns all domains, attributes, and progression points for child development observations";
            s.Response<ClassificationDataVm>(200, "Classification data retrieved successfully");
        });
    }

    public override async Task HandleAsync(GetClassificationDataRequest req, CancellationToken ct)
    {
        var query = new GetClassificationDataQuery
        {
            IncludeInactive = req.IncludeInactive
        };

        var result = await _mediator.Send(query, ct);
        await SendOkAsync(result, ct);
    }
}
