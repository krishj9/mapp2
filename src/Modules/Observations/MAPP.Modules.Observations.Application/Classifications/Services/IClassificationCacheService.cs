using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;

namespace MAPP.Modules.Observations.Application.Classifications.Services;

/// <summary>
/// Service for caching classification data for fast retrieval
/// </summary>
public interface IClassificationCacheService
{
    /// <summary>
    /// Gets cached classification data
    /// </summary>
    Task<ClassificationDataVm?> GetCachedClassificationDataAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets classification data in cache
    /// </summary>
    Task SetCachedClassificationDataAsync(ClassificationDataVm data, bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all cached classification data
    /// </summary>
    Task InvalidateCacheAsync(CancellationToken cancellationToken = default);
}
