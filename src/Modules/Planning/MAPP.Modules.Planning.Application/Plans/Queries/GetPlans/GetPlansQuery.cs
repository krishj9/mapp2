using MediatR;

namespace MAPP.Modules.Planning.Application.Plans.Queries.GetPlans;

/// <summary>
/// Get plans query following CQRS pattern from Ardalis template
/// </summary>
public record GetPlansQuery : IRequest<PlansVm>
{
    public int? Status { get; init; }
    public int? Priority { get; init; }
    public string? OwnerId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
