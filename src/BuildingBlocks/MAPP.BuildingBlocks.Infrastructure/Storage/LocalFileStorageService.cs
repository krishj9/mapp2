using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Application.Common.Models;
using System.Security.Cryptography;
using System.Text.Json;

namespace MAPP.BuildingBlocks.Infrastructure.Storage;

/// <summary>
/// Local file system implementation of media storage service for development
/// </summary>
public class LocalFileStorageService : IMediaStorageService
{
    private readonly MediaStorageOptions _options;
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _baseStoragePath;

    public LocalFileStorageService(
        IOptions<MediaStorageOptions> options,
        ILogger<LocalFileStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _baseStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "LocalStorage");
        
        // Ensure storage directory exists
        Directory.CreateDirectory(_baseStoragePath);
    }

    public async Task<Result<MediaUploadResult>> UploadAsync(MediaUploadRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            var validationResult = ValidateUploadRequest(request);
            if (!validationResult.Succeeded)
            {
                return Result<MediaUploadResult>.Failure(validationResult.Errors);
            }

            var bucketName = request.BucketName ?? _options.DefaultBucketName;
            var fileName = GenerateFileName(request);
            var filePath = GenerateFilePath(request, fileName);
            var fullPath = Path.Combine(_baseStoragePath, bucketName, filePath);

            _logger.LogInformation("[LOCAL STORAGE] Uploading file {FileName} to {FullPath}", fileName, fullPath);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write file to disk
            await File.WriteAllBytesAsync(fullPath, request.FileContent, cancellationToken);

            // Create metadata file
            var metadataPath = fullPath + ".metadata.json";
            var metadata = new
            {
                OriginalFileName = request.FileName,
                ContentType = request.ContentType,
                UploadedAt = DateTime.UtcNow,
                FileSizeBytes = request.FileContent.Length,
                CustomMetadata = request.Metadata,
                CacheControl = request.CacheControl,
                IsPublic = request.IsPublic
            };

            await File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);

            var result = new MediaUploadResult
            {
                FileId = Guid.NewGuid().ToString(),
                FileName = fileName,
                OriginalFileName = request.FileName,
                BucketName = bucketName,
                FilePath = filePath,
                PublicUrl = request.IsPublic ? $"http://localhost/storage/{bucketName}/{filePath}" : null,
                FileSizeBytes = request.FileContent.Length,
                ContentType = request.ContentType,
                UploadedAt = DateTime.UtcNow,
                ETag = ComputeETag(request.FileContent),
                Metadata = request.Metadata
            };

            _logger.LogInformation("[LOCAL STORAGE] Successfully uploaded file {FileName}", fileName);
            return Result<MediaUploadResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LOCAL STORAGE] Failed to upload file {FileName}", request.FileName);
            return Result<MediaUploadResult>.Failure($"Failed to upload file: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<MediaUploadResult>>> UploadBatchAsync(IEnumerable<MediaUploadRequest> requests, CancellationToken cancellationToken = default)
    {
        var results = new List<MediaUploadResult>();
        var errors = new List<string>();

        foreach (var request in requests)
        {
            var uploadResult = await UploadAsync(request, cancellationToken);
            if (uploadResult.Succeeded)
            {
                results.Add(uploadResult.Data!);
            }
            else
            {
                errors.AddRange(uploadResult.Errors);
            }
        }

        if (errors.Any())
        {
            return Result<IEnumerable<MediaUploadResult>>.Failure(errors);
        }

        return Result<IEnumerable<MediaUploadResult>>.Success(results);
    }

    public Task<Result<string>> GenerateDownloadUrlAsync(string fileName, string? bucketName = null, int expirationMinutes = 60, CancellationToken cancellationToken = default)
    {
        try
        {
            bucketName ??= _options.DefaultBucketName;
            
            // Generate a simple signed URL for local development
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);
            var signedUrl = $"http://localhost/storage/{bucketName}/{fileName}?expires={expiration:yyyy-MM-ddTHH:mm:ssZ}";

            _logger.LogDebug("[LOCAL STORAGE] Generated signed URL for file {FileName}: {SignedUrl}", fileName, signedUrl);

            return Task.FromResult(Result<string>.Success(signedUrl));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LOCAL STORAGE] Failed to generate download URL for file {FileName}", fileName);
            return Task.FromResult(Result<string>.Failure($"Failed to generate download URL: {ex.Message}"));
        }
    }

    public async Task<Result<IDictionary<string, string>>> GenerateDownloadUrlsBatchAsync(IEnumerable<string> fileNames, string? bucketName = null, int expirationMinutes = 60, CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, string>();
        var errors = new List<string>();

        foreach (var fileName in fileNames)
        {
            var urlResult = await GenerateDownloadUrlAsync(fileName, bucketName, expirationMinutes, cancellationToken);
            if (urlResult.Succeeded)
            {
                results[fileName] = urlResult.Data!;
            }
            else
            {
                errors.AddRange(urlResult.Errors);
            }
        }

        if (errors.Any())
        {
            return Result<IDictionary<string, string>>.Failure(errors);
        }

        return Result<IDictionary<string, string>>.Success(results);
    }

    public Task<Result> DeleteAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            bucketName ??= _options.DefaultBucketName;
            var fullPath = Path.Combine(_baseStoragePath, bucketName, fileName);

            _logger.LogInformation("[LOCAL STORAGE] Deleting file {FileName} from {FullPath}", fileName, fullPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                
                // Also delete metadata file if it exists
                var metadataPath = fullPath + ".metadata.json";
                if (File.Exists(metadataPath))
                {
                    File.Delete(metadataPath);
                }
            }

            _logger.LogInformation("[LOCAL STORAGE] Successfully deleted file {FileName}", fileName);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LOCAL STORAGE] Failed to delete file {FileName}", fileName);
            return Task.FromResult(Result.Failure($"Failed to delete file: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteBatchAsync(IEnumerable<string> fileNames, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        foreach (var fileName in fileNames)
        {
            var deleteResult = await DeleteAsync(fileName, bucketName, cancellationToken);
            if (!deleteResult.Succeeded)
            {
                errors.AddRange(deleteResult.Errors);
            }
        }

        if (errors.Any())
        {
            return Result.Failure(errors);
        }

        return Result.Success();
    }

    public Task<Result<bool>> ExistsAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            bucketName ??= _options.DefaultBucketName;
            var fullPath = Path.Combine(_baseStoragePath, bucketName, fileName);
            var exists = File.Exists(fullPath);

            return Task.FromResult(Result<bool>.Success(exists));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LOCAL STORAGE] Failed to check if file {FileName} exists", fileName);
            return Task.FromResult(Result<bool>.Failure($"Failed to check file existence: {ex.Message}"));
        }
    }

    public async Task<Result<MediaFileMetadata>> GetMetadataAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            bucketName ??= _options.DefaultBucketName;
            var fullPath = Path.Combine(_baseStoragePath, bucketName, fileName);
            var metadataPath = fullPath + ".metadata.json";

            if (!File.Exists(fullPath))
            {
                return Result<MediaFileMetadata>.Failure("File not found");
            }

            var fileInfo = new FileInfo(fullPath);
            var metadata = new MediaFileMetadata
            {
                FileName = Path.GetFileName(fileName),
                BucketName = bucketName,
                FilePath = fileName,
                FileSizeBytes = fileInfo.Length,
                ContentType = "application/octet-stream",
                CreatedAt = fileInfo.CreationTimeUtc,
                LastModified = fileInfo.LastWriteTimeUtc,
                ETag = ComputeETag(await File.ReadAllBytesAsync(fullPath, cancellationToken))
            };

            // Load additional metadata if available
            if (File.Exists(metadataPath))
            {
                var metadataJson = await File.ReadAllTextAsync(metadataPath, cancellationToken);
                var additionalMetadata = JsonSerializer.Deserialize<Dictionary<string, object>>(metadataJson);
                
                if (additionalMetadata != null)
                {
                    if (additionalMetadata.TryGetValue("ContentType", out var contentType))
                    {
                        metadata = metadata with { ContentType = contentType.ToString() ?? metadata.ContentType };
                    }

                    if (additionalMetadata.TryGetValue("CustomMetadata", out var customMetadata))
                    {
                        var customMetadataDict = JsonSerializer.Deserialize<Dictionary<string, string>>(customMetadata.ToString() ?? "{}");
                        metadata = metadata with { Metadata = customMetadataDict };
                    }
                }
            }

            return Result<MediaFileMetadata>.Success(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[LOCAL STORAGE] Failed to get metadata for file {FileName}", fileName);
            return Result<MediaFileMetadata>.Failure($"Failed to get file metadata: {ex.Message}");
        }
    }

    private Result ValidateUploadRequest(MediaUploadRequest request)
    {
        var errors = new List<string>();

        if (request.FileContent == null || request.FileContent.Length == 0)
        {
            errors.Add("File content cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            errors.Add("File name is required");
        }

        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            errors.Add("Content type is required");
        }

        if (request.FileContent?.Length > _options.MaxFileSizeBytes)
        {
            errors.Add($"File size exceeds maximum allowed size of {_options.MaxFileSizeBytes} bytes");
        }

        var fileExtension = Path.GetExtension(request.FileName)?.ToLowerInvariant();
        if (!string.IsNullOrEmpty(fileExtension) && !_options.AllowedFileExtensions.Contains(fileExtension))
        {
            errors.Add($"File extension '{fileExtension}' is not allowed");
        }

        if (!_options.AllowedMimeTypes.Contains(request.ContentType))
        {
            errors.Add($"MIME type '{request.ContentType}' is not allowed");
        }

        return errors.Any() ? Result.Failure(errors) : Result.Success();
    }

    private string GenerateFileName(MediaUploadRequest request)
    {
        if (!request.GenerateUniqueFileName)
        {
            return request.FileName;
        }

        var extension = Path.GetExtension(request.FileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(request.FileName);
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        return $"{nameWithoutExtension}_{timestamp}_{uniqueId}{extension}";
    }

    private string GenerateFilePath(MediaUploadRequest request, string fileName)
    {
        if (!string.IsNullOrEmpty(request.FolderPath))
        {
            return $"{request.FolderPath.Trim('/')}/{fileName}";
        }

        // Use default folder structure
        var now = DateTime.UtcNow;
        var folderStructure = _options.DefaultFolderStructure
            .Replace("{domain}", "media")
            .Replace("{year}", now.Year.ToString())
            .Replace("{month}", now.Month.ToString("D2"));

        return $"{folderStructure}/{fileName}";
    }

    private static string ComputeETag(byte[] content)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(content);
        return Convert.ToBase64String(hash);
    }
}
