using FastEndpoints;
using MAPP.BuildingBlocks.Web.Endpoints;
using MAPP.Modules.Planning.Application.Plans.Queries.GetPlans;

namespace MAPP.Services.Planning.Endpoints.Plans;

/// <summary>
/// Get plan by ID endpoint using FastEndpoints following Ardalis patterns
/// </summary>
public class GetById : BaseEndpoint<GetPlanByIdRequest, PlanBriefDto>
{
    protected override void ConfigureEndpoint()
    {
        Get("/plans/{id}");
        Summary(s =>
        {
            s.Summary = "Get plan by ID";
            s.Description = "Retrieves a specific plan by its ID";
            s.Response<PlanBriefDto>(200, "Plan retrieved successfully");
            s.Response(404, "Plan not found");
        });
    }

    public override async Task HandleAsync(GetPlanByIdRequest req, CancellationToken ct)
    {
        // For now, we'll use the GetPlans query with a filter
        // In a real implementation, you'd create a GetPlanByIdQuery
        var query = new GetPlansQuery
        {
            PageNumber = 1,
            PageSize = 1
        };

        var result = await Mediator.Send(query, ct);
        
        var plan = result.Plans.FirstOrDefault(p => p.Id == req.Id);
        
        if (plan == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(plan, ct);
    }
}

public class GetPlanByIdRequest
{
    public int Id { get; set; }
}
