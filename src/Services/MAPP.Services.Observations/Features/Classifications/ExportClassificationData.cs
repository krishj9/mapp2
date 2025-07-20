using FastEndpoints;
using MAPP.Modules.Observations.Application.Classifications.Services;

namespace MAPP.Services.Observations.Features.Classifications;

/// <summary>
/// Response model for export operation
/// </summary>
public class ExportClassificationDataResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? FileName { get; set; }
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Fast Endpoint for exporting classification data to cloud storage
/// </summary>
public class ExportClassificationDataEndpoint : EndpointWithoutRequest<ExportClassificationDataResponse>
{
    private readonly IClassificationDataService _classificationService;

    public ExportClassificationDataEndpoint(IClassificationDataService classificationService)
    {
        _classificationService = classificationService;
    }

    public override void Configure()
    {
        Post("/api/observations/classifications/export");
        AllowAnonymous(); // No authentication required as per requirements
        Summary(s =>
        {
            s.Summary = "Export classification data to cloud storage";
            s.Description = "Exports current database classification data to Google Cloud Storage with versioning";
            s.Response<ExportClassificationDataResponse>(200, "Export completed successfully");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var success = await _classificationService.ExportToCloudStorageAsync(ct);
            
            if (success)
            {
                var version = await _classificationService.GetDataVersionAsync(ct);
                
                var response = new ExportClassificationDataResponse
                {
                    Success = true,
                    Message = "Classification data exported to cloud storage successfully",
                    Version = version,
                    FileName = version != null ? $"classification-data-{version}.json" : null,
                    ExportedAt = DateTime.UtcNow
                };

                await SendOkAsync(response, ct);
            }
            else
            {
                var response = new ExportClassificationDataResponse
                {
                    Success = false,
                    Message = "Failed to export classification data to cloud storage",
                    ExportedAt = DateTime.UtcNow
                };

                await SendOkAsync(response, ct);
            }
        }
        catch (Exception ex)
        {
            var response = new ExportClassificationDataResponse
            {
                Success = false,
                Message = $"Export failed with error: {ex.Message}",
                ExportedAt = DateTime.UtcNow
            };

            await SendOkAsync(response, ct);
        }
    }
}
