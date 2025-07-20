using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;

namespace MAPP.Modules.Observations.Application.Classifications.Services;

/// <summary>
/// Enhanced service for managing classification data with cloud storage and caching
/// </summary>
public interface IClassificationDataService
{
    /// <summary>
    /// Gets classification data with multi-tier caching (Redis → GCS → Database)
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete classification data</returns>
    Task<ClassificationDataVm> GetClassificationDataAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces refresh of cached data from cloud storage or database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refreshed classification data</returns>
    Task<ClassificationDataVm> RefreshCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current data version timestamp
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Version timestamp or null if no data exists</returns>
    Task<string?> GetDataVersionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports current database data to cloud storage with new version
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result with new version</returns>
    Task<bool> ExportToCloudStorageAsync(CancellationToken cancellationToken = default);
}
