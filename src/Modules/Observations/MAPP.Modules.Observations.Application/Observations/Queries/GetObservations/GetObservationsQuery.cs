using MediatR;

namespace MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

/// <summary>
/// Get observations query following CQRS pattern from Ardalis template
/// </summary>
public record GetObservationsQuery : IRequest<ObservationsVm>
{
    public int? Status { get; init; }
    public int? Priority { get; init; }
    public string? ObserverId { get; init; }
    public string? Location { get; init; }
    public DateTimeOffset? FromDate { get; init; }
    public DateTimeOffset? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
