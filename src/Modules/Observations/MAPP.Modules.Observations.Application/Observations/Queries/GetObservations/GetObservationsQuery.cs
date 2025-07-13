using MediatR;

namespace MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

/// <summary>
/// Get observations query following CQRS pattern from Ardalis template
/// </summary>
public record GetObservationsQuery : IRequest<ObservationsVm>
{
    public long? ChildId { get; init; }
    public long? TeacherId { get; init; }
    public int? DomainId { get; init; }
    public int? AttributeId { get; init; }
    public bool? IsDraft { get; init; }
    public string? SearchText { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
