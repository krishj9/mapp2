using MediatR;

namespace MAPP.Modules.Observations.Application.Observations.Commands.CreateObservation;

/// <summary>
/// Create observation command following CQRS pattern from Ardalis template
/// </summary>
public record CreateObservationCommand : IRequest<int>
{
    public long ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
    public long TeacherId { get; init; }
    public string TeacherName { get; init; } = string.Empty;
    public int DomainId { get; init; }
    public string DomainName { get; init; } = string.Empty;
    public int AttributeId { get; init; }
    public string AttributeName { get; init; } = string.Empty;
    public List<int> ProgressionPointIds { get; init; } = new();
    public string ObservationText { get; init; } = string.Empty;
    public DateTime ObservationDate { get; init; }
    public string? LearningContext { get; init; }
    public List<string> Tags { get; init; } = new();
    public bool IsDraft { get; init; } = false;
}
