using FastEndpoints;
using MAPP.BuildingBlocks.Web.Endpoints;
using MAPP.Modules.Planning.Application.Plans.Commands.CreatePlan;

namespace MAPP.Services.Planning.Endpoints.Plans;

/// <summary>
/// Create plan endpoint using FastEndpoints following clean architecture patterns
/// </summary>
public class Create : BaseEndpoint<CreatePlanRequest, CreatePlanResponse>
{
    protected override void ConfigureEndpoint()
    {
        Post("/plans");
        Summary(s =>
        {
            s.Summary = "Create a new plan";
            s.Description = "Creates a new plan with the specified details";
            s.Response<CreatePlanResponse>(201, "Plan created successfully");
            s.Response(400, "Invalid request");
        });
    }

    public override async Task HandleAsync(CreatePlanRequest req, CancellationToken ct)
    {
        var command = new CreatePlanCommand
        {
            Title = req.Title,
            Description = req.Description,
            Priority = req.Priority,
            StartDate = req.StartDate,
            EndDate = req.EndDate
        };

        var planId = await Mediator.Send(command, ct);

        await SendCreatedAtAsync<GetById>(new { id = planId }, new CreatePlanResponse { Id = planId }, cancellation: ct);
    }
}

public class CreatePlanRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; } = 2; // Medium priority
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}

public class CreatePlanResponse
{
    public int Id { get; set; }
}
