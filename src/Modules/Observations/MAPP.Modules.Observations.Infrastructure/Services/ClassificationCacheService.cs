using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;
using MAPP.Modules.Observations.Application.Classifications.Services;

namespace MAPP.Modules.Observations.Infrastructure.Services;

/// <summary>
/// Redis-based caching service for classification data
/// </summary>
public class ClassificationCacheService : IClassificationCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ClassificationCacheService> _logger;
    
    private const string CacheKeyActive = "observations:classification:active";
    private const string CacheKeyAll = "observations:classification:all";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24); // Cache for 24 hours

    public ClassificationCacheService(
        IDistributedCache cache,
        ILogger<ClassificationCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<ClassificationDataVm?> GetCachedClassificationDataAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = includeInactive ? CacheKeyAll : CacheKeyActive;
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogDebug("Classification data cache miss for key: {CacheKey}", cacheKey);
                return null;
            }

            var data = JsonSerializer.Deserialize<ClassificationDataVm>(cachedData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogDebug("Classification data cache hit for key: {CacheKey}, domains count: {DomainsCount}", 
                cacheKey, data?.Domains?.Count ?? 0);

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classification data from cache");
            return null;
        }
    }

    public async Task SetCachedClassificationDataAsync(ClassificationDataVm data, bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = includeInactive ? CacheKeyAll : CacheKeyActive;
            var serializedData = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration
            };

            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

            _logger.LogInformation("Classification data cached successfully for key: {CacheKey}, domains count: {DomainsCount}", 
                cacheKey, data.Domains.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching classification data for key: {CacheKey}", 
                includeInactive ? CacheKeyAll : CacheKeyActive);
        }
    }

    public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(CacheKeyActive, cancellationToken);
            await _cache.RemoveAsync(CacheKeyAll, cancellationToken);

            _logger.LogInformation("Classification data cache invalidated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating classification data cache");
        }
    }
}
