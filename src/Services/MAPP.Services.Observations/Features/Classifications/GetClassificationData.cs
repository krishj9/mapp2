using FastEndpoints;
using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;
using MAPP.Modules.Observations.Application.Classifications.Services;

namespace MAPP.Services.Observations.Features.Classifications;

/// <summary>
/// Request model for getting classification data
/// </summary>
public class GetClassificationDataRequest
{
    public bool IncludeInactive { get; set; } = false;
}

/// <summary>
/// Fast Endpoint for getting complete classification data with enhanced caching
/// </summary>
public class GetClassificationDataEndpoint : Endpoint<GetClassificationDataRequest, ClassificationDataVm>
{
    private readonly IClassificationDataService _classificationService;

    public GetClassificationDataEndpoint(IClassificationDataService classificationService)
    {
        _classificationService = classificationService;
    }

    public override void Configure()
    {
        Get("/api/observations/classifications");
        AllowAnonymous(); // TODO: Add proper authorization
        Summary(s =>
        {
            s.Summary = "Get complete classification data";
            s.Description = "Returns all domains, attributes, and progression points with multi-tier caching (Redis → GCS → Database)";
            s.Response<ClassificationDataVm>(200, "Classification data retrieved successfully");
        });
    }

    public override async Task HandleAsync(GetClassificationDataRequest req, CancellationToken ct)
    {
        var result = await _classificationService.GetClassificationDataAsync(req.IncludeInactive, ct);
        await SendOkAsync(result, ct);
    }
}
