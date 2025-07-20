using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Application.Common.Models;
using MAPP.Modules.Observations.Application.Classifications.Models;
using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;
using MAPP.Modules.Observations.Application.Classifications.Services;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Infrastructure.Services;

namespace MAPP.Modules.Observations.Infrastructure.Services;

/// <summary>
/// Enhanced classification data service with multi-tier caching (Redis → GCS → Database)
/// </summary>
public class ClassificationDataService : IClassificationDataService
{
    private readonly IObservationsDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IMediaStorageService _storageService;
    private readonly ILogger<ClassificationDataService> _logger;
    
    private const string CacheKeyActive = "observations:classification:active";
    private const string CacheKeyAll = "observations:classification:all";
    private const string CacheKeyVersion = "observations:classification:version";
    private const string BucketName = "mapp-classification-data";
    private const string FolderPath = "classification-data";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);

    public ClassificationDataService(
        IObservationsDbContext context,
        IDistributedCache cache,
        IMediaStorageService storageService,
        ILogger<ClassificationDataService> logger)
    {
        _context = context;
        _cache = cache;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<ClassificationDataVm> GetClassificationDataAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        // Step 1: Try Redis cache
        var cachedData = await GetFromCacheAsync(includeInactive, cancellationToken);
        if (cachedData != null)
        {
            _logger.LogDebug("Classification data retrieved from Redis cache, domains count: {DomainsCount}", cachedData.Domains.Count);
            return cachedData;
        }

        // Step 2: Try cloud storage
        var cloudData = await GetFromCloudStorageAsync(includeInactive, cancellationToken);
        if (cloudData != null)
        {
            await SetCacheAsync(cloudData, includeInactive, cancellationToken);
            _logger.LogInformation("Classification data retrieved from cloud storage and cached, domains count: {DomainsCount}", cloudData.Domains.Count);
            return cloudData;
        }

        // Step 3: Fallback to database
        var databaseData = await GetFromDatabaseAsync(includeInactive, cancellationToken);
        
        // Cache the database result
        await SetCacheAsync(databaseData, includeInactive, cancellationToken);
        
        // Export to cloud storage for future requests
        _ = Task.Run(async () => await ExportToCloudStorageAsync(cancellationToken), cancellationToken);
        
        _logger.LogInformation("Classification data retrieved from database and cached, domains count: {DomainsCount}", databaseData.Domains.Count);
        return databaseData;
    }

    public async Task<ClassificationDataVm> RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refreshing classification data cache");
        
        // Clear Redis cache
        await _cache.RemoveAsync(CacheKeyActive, cancellationToken);
        await _cache.RemoveAsync(CacheKeyAll, cancellationToken);
        await _cache.RemoveAsync(CacheKeyVersion, cancellationToken);
        
        // Get fresh data (will rebuild cache)
        return await GetClassificationDataAsync(false, cancellationToken);
    }

    public async Task<string?> GetDataVersionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedVersion = await _cache.GetStringAsync(CacheKeyVersion, cancellationToken);
            if (!string.IsNullOrEmpty(cachedVersion))
            {
                return cachedVersion;
            }

            // Try to get version from latest cloud storage file
            var latestFileName = await GetLatestCloudFileNameAsync(cancellationToken);
            if (!string.IsNullOrEmpty(latestFileName))
            {
                // Extract timestamp from filename: classification-data-20250114-103000.json
                var timestamp = latestFileName.Replace("classification-data-", "").Replace(".json", "");
                await _cache.SetStringAsync(CacheKeyVersion, timestamp, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                }, cancellationToken);
                return timestamp;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data version");
            return null;
        }
    }

    public async Task<bool> ExportToCloudStorageAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting export of classification data to cloud storage");

            // Get current data from database
            var data = await GetFromDatabaseAsync(false, cancellationToken);
            
            // Create file model
            var fileData = new ClassificationDataFile
            {
                Version = "1.0",
                LastUpdated = DateTime.UtcNow,
                Source = "MAPP Database Export",
                Domains = MapToFileModel(data)
            };

            // Serialize to JSON
            var jsonContent = JsonSerializer.Serialize(fileData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            // Generate filename with timestamp
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            var fileName = $"classification-data-{timestamp}.json";

            // Upload to cloud storage
            var uploadRequest = new MediaUploadRequest
            {
                FileContent = System.Text.Encoding.UTF8.GetBytes(jsonContent),
                FileName = fileName,
                ContentType = "application/json",
                BucketName = BucketName,
                FolderPath = FolderPath,
                GenerateUniqueFileName = false,
                IsPublic = false,
                Metadata = new Dictionary<string, string>
                {
                    ["version"] = "1.0",
                    ["exportedAt"] = DateTime.UtcNow.ToString("O"),
                    ["domainsCount"] = data.Domains.Count.ToString()
                }
            };

            var result = await _storageService.UploadAsync(uploadRequest, cancellationToken);
            
            if (result.Succeeded)
            {
                // Update version cache
                await _cache.SetStringAsync(CacheKeyVersion, timestamp, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                }, cancellationToken);

                _logger.LogInformation("Successfully exported classification data to cloud storage: {FileName}", fileName);
                return true;
            }
            else
            {
                _logger.LogError("Failed to export classification data: {Errors}", string.Join(", ", result.Errors));
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting classification data to cloud storage");
            return false;
        }
    }

    private async Task<ClassificationDataVm?> GetFromCacheAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = includeInactive ? CacheKeyAll : CacheKeyActive;
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (string.IsNullOrEmpty(cachedData))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ClassificationDataVm>(cachedData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classification data from cache");
            return null;
        }
    }

    private async Task SetCacheAsync(ClassificationDataVm data, bool includeInactive, CancellationToken cancellationToken)
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
            _logger.LogDebug("Classification data cached successfully for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching classification data");
        }
    }

    private async Task<ClassificationDataVm?> GetFromCloudStorageAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        try
        {
            var fileName = await GetLatestCloudFileNameAsync(cancellationToken);
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogDebug("No classification data file found in cloud storage");
                return null;
            }

            // For now, cloud storage retrieval is not fully implemented
            // This would require downloading and parsing the JSON file
            _logger.LogDebug("Found classification data file in cloud storage: {FileName}", fileName);
            return null; // TODO: Implement actual file download and parsing
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classification data from cloud storage");
            return null;
        }
    }

    private async Task<string?> GetLatestCloudFileNameAsync(CancellationToken cancellationToken)
    {
        // This would need to be implemented to list files in the bucket
        // and find the latest classification-data-*.json file
        // For now, return null to fall back to database
        return await Task.FromResult<string?>(null);
    }

    private async Task<ClassificationDataVm> GetFromDatabaseAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var domainsQuery = _context.ObservationDomains
            .Include(d => d.Attributes.Where(a => includeInactive || a.IsActive))
                .ThenInclude(a => a.ProgressionPoints.Where(p => includeInactive || p.IsActive))
            .Where(d => includeInactive || d.IsActive)
            .OrderBy(d => d.SortOrder);

        var domains = await domainsQuery.ToListAsync(cancellationToken);

        return new ClassificationDataVm
        {
            Domains = domains.Select(d => new DomainDto
            {
                Id = d.Id,
                Name = d.Name,
                CategoryName = d.CategoryName,
                CategoryTitle = d.CategoryTitle,
                SortOrder = d.SortOrder,
                Attributes = d.Attributes
                    .OrderBy(a => a.SortOrder)
                    .Select(a => new AttributeDto
                    {
                        Id = a.Id,
                        Number = a.Number,
                        Name = a.Name,
                        CategoryInformation = a.CategoryInformation,
                        SortOrder = a.SortOrder,
                        ProgressionPoints = a.ProgressionPoints
                            .OrderBy(p => p.SortOrder)
                            .Select(p => new ProgressionPointDto
                            {
                                ProgressionId = p.Id,
                                Points = p.Points,
                                Title = p.Title,
                                Description = p.Description,
                                Order = p.Order,
                                CategoryInformation = p.CategoryInformation,
                                SortOrder = p.SortOrder
                            }).ToList()
                    }).ToList()
            }).ToList()
        };
    }

    private List<DomainFileModel> MapToFileModel(ClassificationDataVm data)
    {
        return data.Domains.Select(d => new DomainFileModel
        {
            Id = d.Id,
            Name = d.Name,
            CategoryName = d.CategoryName,
            CategoryTitle = d.CategoryTitle,
            SortOrder = d.SortOrder,
            IsActive = true, // Only active items are exported
            Attributes = d.Attributes.Select(a => new AttributeFileModel
            {
                Id = a.Id,
                Number = a.Number,
                Name = a.Name,
                CategoryInformation = a.CategoryInformation,
                SortOrder = a.SortOrder,
                IsActive = true, // Only active items are exported
                ProgressionPoints = a.ProgressionPoints.Select(p => new ProgressionPointFileModel
                {
                    Id = p.ProgressionId,
                    Points = p.Points,
                    Title = p.Title,
                    Description = p.Description,
                    Order = p.Order,
                    CategoryInformation = p.CategoryInformation,
                    SortOrder = p.SortOrder,
                    IsActive = true // Only active items are exported
                }).ToList()
            }).ToList()
        }).ToList();
    }
}
