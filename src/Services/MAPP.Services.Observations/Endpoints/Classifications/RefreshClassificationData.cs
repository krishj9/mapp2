using FastEndpoints;
using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;
using MAPP.Modules.Observations.Application.Classifications.Services;

namespace MAPP.Services.Observations.Endpoints.Classifications;

/// <summary>
/// Response model for refresh operation
/// </summary>
public class RefreshClassificationDataResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ClassificationDataVm? Data { get; set; }
}

/// <summary>
/// Fast Endpoint for refreshing classification data cache
/// </summary>
public class RefreshClassificationDataEndpoint : EndpointWithoutRequest<RefreshClassificationDataResponse>
{
    private readonly IClassificationDataService _classificationService;

    public RefreshClassificationDataEndpoint(IClassificationDataService classificationService)
    {
        _classificationService = classificationService;
    }

    public override void Configure()
    {
        Post("/api/observations/classifications/refresh");
        AllowAnonymous(); // No authentication required as per requirements
        Summary(s =>
        {
            s.Summary = "Refresh classification data cache";
            s.Description = "Forces refresh of cached classification data from cloud storage or database";
            s.Response<RefreshClassificationDataResponse>(200, "Cache refreshed successfully");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var data = await _classificationService.RefreshCacheAsync(ct);
            
            var response = new RefreshClassificationDataResponse
            {
                Success = true,
                Message = "Classification data cache refreshed successfully",
                Data = data
            };

            await SendOkAsync(response, ct);
        }
        catch (Exception ex)
        {
            var response = new RefreshClassificationDataResponse
            {
                Success = false,
                Message = $"Failed to refresh cache: {ex.Message}"
            };

            await SendOkAsync(response, ct);
        }
    }
}
