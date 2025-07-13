using Google.Cloud.Storage.V1;
using Google;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Application.Common.Models;

namespace MAPP.BuildingBlocks.Infrastructure.Storage;

/// <summary>
/// Google Cloud Storage implementation of media storage service
/// </summary>
public class GoogleCloudStorageService : IMediaStorageService
{
    private readonly StorageClient _storageClient;
    private readonly MediaStorageOptions _options;
    private readonly ILogger<GoogleCloudStorageService> _logger;

    public GoogleCloudStorageService(
        StorageClient storageClient,
        IOptions<MediaStorageOptions> options,
        ILogger<GoogleCloudStorageService> logger)
    {
        _storageClient = storageClient;
        _options = options.Value;
        _logger = logger;
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

            _logger.LogInformation("Uploading file {FileName} to bucket {BucketName} at path {FilePath}", 
                fileName, bucketName, filePath);

            // Upload the file directly
            using var stream = new MemoryStream(request.FileContent);
            var uploadedObject = await _storageClient.UploadObjectAsync(
                bucketName,
                filePath,
                request.ContentType,
                stream,
                cancellationToken: cancellationToken);

            // Set public access if requested
            if (request.IsPublic)
            {
                await MakeObjectPublicAsync(bucketName, filePath, cancellationToken);
            }

            var result = new MediaUploadResult
            {
                FileId = uploadedObject.Id,
                FileName = fileName,
                OriginalFileName = request.FileName,
                BucketName = bucketName,
                FilePath = filePath,
                PublicUrl = request.IsPublic ? $"https://storage.googleapis.com/{bucketName}/{filePath}" : null,
                FileSizeBytes = request.FileContent.Length,
                ContentType = request.ContentType,
                UploadedAt = DateTime.UtcNow,
                ETag = uploadedObject.ETag,
                Metadata = request.Metadata
            };

            _logger.LogInformation("Successfully uploaded file {FileName} with ID {FileId}", fileName, uploadedObject.Id);
            return Result<MediaUploadResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName}", request.FileName);
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
            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            _logger.LogDebug("Generating signed URL for file {FileName} in bucket {BucketName}, expires at {Expiration}",
                fileName, bucketName, expiration);

            // For now, return a simple URL - in production you'd use UrlSigner with service account credentials
            var signedUrl = $"https://storage.googleapis.com/{bucketName}/{fileName}?expires={expiration:yyyy-MM-ddTHH:mm:ssZ}";

            return Task.FromResult(Result<string>.Success(signedUrl));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate download URL for file {FileName}", fileName);
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

    public async Task<Result> DeleteAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            bucketName ??= _options.DefaultBucketName;

            _logger.LogInformation("Deleting file {FileName} from bucket {BucketName}", fileName, bucketName);

            await _storageClient.DeleteObjectAsync(bucketName, fileName, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully deleted file {FileName}", fileName);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FileName}", fileName);
            return Result.Failure($"Failed to delete file: {ex.Message}");
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

    public async Task<Result<bool>> ExistsAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            bucketName ??= _options.DefaultBucketName;

            var obj = await _storageClient.GetObjectAsync(bucketName, fileName, cancellationToken: cancellationToken);
            return Result<bool>.Success(obj != null);
        }
        catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if file {FileName} exists", fileName);
            return Result<bool>.Failure($"Failed to check file existence: {ex.Message}");
        }
    }

    public async Task<Result<MediaFileMetadata>> GetMetadataAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            bucketName ??= _options.DefaultBucketName;

            var obj = await _storageClient.GetObjectAsync(bucketName, fileName, cancellationToken: cancellationToken);

            var metadata = new MediaFileMetadata
            {
                FileName = Path.GetFileName(obj.Name),
                BucketName = obj.Bucket,
                FilePath = obj.Name,
                FileSizeBytes = (long)(obj.Size ?? 0),
                ContentType = obj.ContentType ?? "application/octet-stream",
                CreatedAt = obj.TimeCreatedDateTimeOffset?.DateTime ?? DateTime.MinValue,
                LastModified = obj.UpdatedDateTimeOffset?.DateTime ?? DateTime.MinValue,
                ETag = obj.ETag,
                MD5Hash = obj.Md5Hash,
                Metadata = obj.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                CacheControl = obj.CacheControl
            };

            return Result<MediaFileMetadata>.Success(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get metadata for file {FileName}", fileName);
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
            .Replace("{domain}", "media") // Default domain
            .Replace("{year}", now.Year.ToString())
            .Replace("{month}", now.Month.ToString("D2"));

        return $"{folderStructure}/{fileName}";
    }

    private async Task MakeObjectPublicAsync(string bucketName, string objectName, CancellationToken cancellationToken)
    {
        try
        {
            // This would require additional IAM permissions
            // For now, we'll log that public access was requested
            _logger.LogInformation("Public access requested for object {ObjectName} in bucket {BucketName}", objectName, bucketName);
            
            // In a production environment, you would implement proper IAM policy updates here
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to make object {ObjectName} public", objectName);
        }
    }
}
