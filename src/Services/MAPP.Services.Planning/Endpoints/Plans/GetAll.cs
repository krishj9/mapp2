using FastEndpoints;
using MAPP.BuildingBlocks.Web.Endpoints;
using MAPP.Modules.Planning.Application.Plans.Queries.GetPlans;

namespace MAPP.Services.Planning.Endpoints.Plans;

/// <summary>
/// Get all plans endpoint using FastEndpoints following Ardalis patterns
/// </summary>
public class GetAll : BaseEndpoint<GetPlansRequest, PlansVm>
{
    protected override void ConfigureEndpoint()
    {
        Get("/plans");
        Summary(s =>
        {
            s.Summary = "Get all plans";
            s.Description = "Retrieves a paginated list of plans with optional filtering";
            s.Response<PlansVm>(200, "Plans retrieved successfully");
        });
    }

    public override async Task HandleAsync(GetPlansRequest req, CancellationToken ct)
    {
        var query = new GetPlansQuery
        {
            Status = req.Status,
            Priority = req.Priority,
            OwnerId = req.OwnerId,
            PageNumber = req.PageNumber,
            PageSize = req.PageSize
        };

        var result = await Mediator.Send(query, ct);

        await SendOkAsync(result, ct);
    }
}

public class GetPlansRequest
{
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public string? OwnerId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
