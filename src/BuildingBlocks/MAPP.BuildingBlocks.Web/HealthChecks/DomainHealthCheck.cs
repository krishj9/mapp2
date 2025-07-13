using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MAPP.BuildingBlocks.Web.HealthChecks;

/// <summary>
/// Base health check for MAPP domains
/// </summary>
public abstract class DomainHealthCheck : IHealthCheck
{
    protected readonly ILogger Logger;
    protected readonly string DomainName;

    protected DomainHealthCheck(ILogger logger, string domainName)
    {
        Logger = logger;
        DomainName = domainName;
    }

    public virtual async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var checks = new Dictionary<string, object>();
            
            // Basic application health
            checks.Add("domain", DomainName);
            checks.Add("timestamp", DateTimeOffset.UtcNow);
            checks.Add("version", GetVersion());
            checks.Add("environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown");

            // Perform domain-specific checks
            var domainChecks = await PerformDomainChecksAsync(cancellationToken);
            foreach (var check in domainChecks)
            {
                checks.Add(check.Key, check.Value);
            }

            // Check if any critical issues
            var hasIssues = domainChecks.Any(c => c.Key.Contains("error") || c.Key.Contains("failed"));
            
            return hasIssues 
                ? HealthCheckResult.Degraded($"{DomainName} domain has some issues", data: checks)
                : HealthCheckResult.Healthy($"{DomainName} domain is healthy", data: checks);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Health check failed for {DomainName}", DomainName);
            return HealthCheckResult.Unhealthy($"{DomainName} domain health check failed", ex);
        }
    }

    protected abstract Task<Dictionary<string, object>> PerformDomainChecksAsync(CancellationToken cancellationToken);

    private string GetVersion()
    {
        return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion 
               ?? Assembly.GetEntryAssembly()?.GetName().Version?.ToString() 
               ?? "Unknown";
    }
}
