using MAPP.BuildingBlocks.Web.HealthChecks;
using MAPP.Modules.Planning.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MAPP.Services.Planning.HealthChecks;

/// <summary>
/// Planning domain specific health check
/// </summary>
public class PlanningHealthCheck : DomainHealthCheck
{
    private readonly IPlanningDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PlanningHealthCheck(
        ILogger<PlanningHealthCheck> logger,
        IPlanningDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) 
        : base(logger, "Planning")
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
                var planCount = await _dbContext.Plans.CountAsync(cancellationToken);
                checks.Add("total_plans", planCount);
                checks.Add("database_status", "healthy");
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
            var aiUrl = _configuration["AI:PlanningAiUrl"];
            if (!string.IsNullOrEmpty(aiUrl))
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                
                var response = await httpClient.GetAsync($"{aiUrl}/health", cancellationToken);
                checks.Add("ai_service_status", response.IsSuccessStatusCode ? "healthy" : "degraded");
                checks.Add("ai_service_response_time", $"{response.Headers.Date}");
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

        // Memory usage check
        var workingSet = Environment.WorkingSet;
        checks.Add("memory_usage_bytes", workingSet);
        checks.Add("memory_usage_mb", Math.Round(workingSet / 1024.0 / 1024.0, 2));

        // Thread pool check
        ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
        checks.Add("available_worker_threads", workerThreads);
        checks.Add("available_completion_port_threads", completionPortThreads);

        return checks;
    }
}
