using FastEndpoints;
using MAPP.BuildingBlocks.Web.Endpoints;
using MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

namespace MAPP.Services.Observations.Endpoints.Observations;

/// <summary>
/// Get all observations endpoint using FastEndpoints following Ardalis patterns
/// </summary>
public class GetAll : BaseEndpoint<GetObservationsRequest, ObservationsVm>
{
    protected override void ConfigureEndpoint()
    {
        Get("/observations");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get all observations";
            s.Description = "Retrieves a paginated list of observations with optional filtering";
            s.Response<ObservationsVm>(200, "Observations retrieved successfully");
        });
    }

    public override async Task HandleAsync(GetObservationsRequest req, CancellationToken ct)
    {
        var query = new GetObservationsQuery
        {
            ChildId = req.ChildId,
            TeacherId = req.TeacherId,
            DomainId = req.DomainId,
            AttributeId = req.AttributeId,
            IsDraft = req.IsDraft,
            SearchText = req.SearchText,
            FromDate = req.FromDate,
            ToDate = req.ToDate,
            PageNumber = req.PageNumber,
            PageSize = req.PageSize
        };

        var result = await Mediator.Send(query, ct);

        await SendOkAsync(result, ct);
    }
}

public class GetObservationsRequest
{
    public long? ChildId { get; set; }
    public long? TeacherId { get; set; }
    public int? DomainId { get; set; }
    public int? AttributeId { get; set; }
    public bool? IsDraft { get; set; }
    public string? SearchText { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
