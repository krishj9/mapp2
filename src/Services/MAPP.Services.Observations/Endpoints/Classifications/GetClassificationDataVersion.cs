using FastEndpoints;
using MAPP.Modules.Observations.Application.Classifications.Services;

namespace MAPP.Services.Observations.Endpoints.Classifications;

/// <summary>
/// Response model for version information
/// </summary>
public class ClassificationDataVersionResponse
{
    public string? Version { get; set; }
    public string? LastUpdated { get; set; }
    public bool HasData { get; set; }
}

/// <summary>
/// Fast Endpoint for getting classification data version
/// </summary>
public class GetClassificationDataVersionEndpoint : EndpointWithoutRequest<ClassificationDataVersionResponse>
{
    private readonly IClassificationDataService _classificationService;

    public GetClassificationDataVersionEndpoint(IClassificationDataService classificationService)
    {
        _classificationService = classificationService;
    }

    public override void Configure()
    {
        Get("/api/observations/classifications/version");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get classification data version";
            s.Description = "Returns the current version timestamp of the classification data";
            s.Response<ClassificationDataVersionResponse>(200, "Version information retrieved successfully");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var version = await _classificationService.GetDataVersionAsync(ct);
        
        var response = new ClassificationDataVersionResponse
        {
            Version = version,
            LastUpdated = version != null ? ParseTimestamp(version) : null,
            HasData = !string.IsNullOrEmpty(version)
        };

        await SendOkAsync(response, ct);
    }

    private string? ParseTimestamp(string version)
    {
        try
        {
            // Parse timestamp format: 20250114-103000
            if (version.Length == 15 && version.Contains('-'))
            {
                var datePart = version.Substring(0, 8);
                var timePart = version.Substring(9, 6);
                
                var year = int.Parse(datePart.Substring(0, 4));
                var month = int.Parse(datePart.Substring(4, 2));
                var day = int.Parse(datePart.Substring(6, 2));
                var hour = int.Parse(timePart.Substring(0, 2));
                var minute = int.Parse(timePart.Substring(2, 2));
                var second = int.Parse(timePart.Substring(4, 2));
                
                var dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
        }
        catch
        {
            // If parsing fails, return the original version
        }
        
        return version;
    }
}
