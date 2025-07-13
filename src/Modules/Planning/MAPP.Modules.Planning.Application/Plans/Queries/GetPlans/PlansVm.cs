namespace MAPP.Modules.Planning.Application.Plans.Queries.GetPlans;

/// <summary>
/// Plans view model following Ardalis patterns
/// </summary>
public class PlansVm
{
    public IList<PlanBriefDto> Plans { get; set; } = new List<PlanBriefDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
