using MediatR;

namespace MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;

/// <summary>
/// Query to get the complete classification data (domains, attributes, progression points)
/// for the UI to load in one response
/// </summary>
public record GetClassificationDataQuery : IRequest<ClassificationDataVm>
{
    public bool IncludeInactive { get; init; } = false;
}
