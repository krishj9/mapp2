using MAPP.BuildingBlocks.Web.HealthChecks;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MAPP.Services.Observations.HealthChecks;

/// <summary>
/// Observations domain specific health check
/// </summary>
public class ObservationsHealthCheck : DomainHealthCheck
{
    private readonly IObservationsDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ObservationsHealthCheck(
        ILogger<ObservationsHealthCheck> logger,
        IObservationsDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) 
        : base(logger, "Observations")
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    protected override async Task<Dictionary<string, object>> PerformDomainChecksAsync(CancellationToken cancellationToken)
    {
        var checks = new Dictionary<string, object>();

        // Database connectivity check
        try
        {
            var dbContext = _dbContext as DbContext;
            var canConnect = await dbContext!.Database.CanConnectAsync(cancellationToken);
            checks.Add("database_connected", canConnect);
            
            if (canConnect)
            {
                var observationCount = await _dbContext.Observations.CountAsync(cancellationToken);
                var artifactCount = await _dbContext.ObservationArtifacts.CountAsync(cancellationToken);
                
                checks.Add("total_observations", observationCount);
                checks.Add("total_artifacts", artifactCount);
                checks.Add("database_status", "healthy");
                
                // Check for recent observations (last 24 hours)
                var recentCount = await _dbContext.Observations
                    .Where(o => o.Created >= DateTimeOffset.UtcNow.AddDays(-1))
                    .CountAsync(cancellationToken);
                checks.Add("recent_observations_24h", recentCount);
            }
            else
            {
                checks.Add("database_status", "failed");
                checks.Add("database_error", "Cannot connect to database");
            }
        }
        catch (Exception ex)
        {
            checks.Add("database_status", "error");
            checks.Add("database_error", ex.Message);
            Logger.LogError(ex, "Database health check failed");
        }

        // AI Service connectivity check
        try
        {
            var aiUrl = _configuration["AI:ObservationsAiUrl"];
            if (!string.IsNullOrEmpty(aiUrl))
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                
                var response = await httpClient.GetAsync($"{aiUrl}/health", cancellationToken);
                checks.Add("ai_service_status", response.IsSuccessStatusCode ? "healthy" : "degraded");
                checks.Add("ai_service_url", aiUrl);
            }
            else
            {
                checks.Add("ai_service_status", "not_configured");
            }
        }
        catch (Exception ex)
        {
            checks.Add("ai_service_status", "error");
            checks.Add("ai_service_error", ex.Message);
            Logger.LogWarning(ex, "AI service health check failed");
        }

        // Data validation metrics
        try
        {
            var publishedCount = await _dbContext.Observations
                .Where(o => !o.IsDraft)
                .CountAsync(cancellationToken);
            var draftCount = await _dbContext.Observations
                .Where(o => o.IsDraft)
                .CountAsync(cancellationToken);
                
            checks.Add("published_observations", publishedCount);
            checks.Add("draft_observations", draftCount);
            
            if (publishedCount + draftCount > 0)
            {
                var publishedRate = (double)publishedCount / (publishedCount + draftCount) * 100;
                checks.Add("published_rate", Math.Round(publishedRate, 2));
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to calculate validation metrics");
        }

        return checks;
    }
}
