using MediatR;

namespace MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

/// <summary>
/// Create observation command following CQRS pattern from Ardalis template
/// </summary>
public record CreateObservationCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Priority { get; init; } = 2; // Medium priority
    public DateTimeOffset? ObservedAt { get; init; }
    public string? Location { get; init; }
}
