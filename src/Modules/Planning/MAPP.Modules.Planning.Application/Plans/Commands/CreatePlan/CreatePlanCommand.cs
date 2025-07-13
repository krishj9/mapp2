using MediatR;
using MAPP.Modules.Planning.Domain.ValueObjects;

namespace MAPP.Modules.Planning.Application.Plans.Commands.CreatePlan;

/// <summary>
/// Create plan command following CQRS pattern from Ardalis template
/// </summary>
public record CreatePlanCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Priority { get; init; } = 2; // Medium priority
    public DateTimeOffset? StartDate { get; init; }
    public DateTimeOffset? EndDate { get; init; }
}
