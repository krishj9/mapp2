namespace MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

/// <summary>
/// Observations view model following Ardalis patterns
/// </summary>
public class ObservationsVm
{
    public IList<ObservationBriefDto> Observations { get; set; } = new List<ObservationBriefDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
