using MAPP.BuildingBlocks.Application.Common.Models;

namespace MAPP.BuildingBlocks.Application.Common.Interfaces;

/// <summary>
/// Service for managing media file uploads and downloads to/from cloud storage
/// </summary>
public interface IMediaStorageService
{
    /// <summary>
    /// Uploads a media file to cloud storage
    /// </summary>
    /// <param name="request">Upload request containing file data and metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload result with file URL and metadata</returns>
    Task<Result<MediaUploadResult>> UploadAsync(MediaUploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads multiple media files to cloud storage
    /// </summary>
    /// <param name="requests">Collection of upload requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of upload results</returns>
    Task<Result<IEnumerable<MediaUploadResult>>> UploadBatchAsync(IEnumerable<MediaUploadRequest> requests, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a signed URL for downloading a media file
    /// </summary>
    /// <param name="fileName">Name of the file in storage</param>
    /// <param name="bucketName">Storage bucket name (optional, uses default if not specified)</param>
    /// <param name="expirationMinutes">URL expiration time in minutes (default: 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Signed URL for file download</returns>
    Task<Result<string>> GenerateDownloadUrlAsync(string fileName, string? bucketName = null, int expirationMinutes = 60, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates signed URLs for downloading multiple media files
    /// </summary>
    /// <param name="fileNames">Collection of file names in storage</param>
    /// <param name="bucketName">Storage bucket name (optional, uses default if not specified)</param>
    /// <param name="expirationMinutes">URL expiration time in minutes (default: 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of signed URLs mapped to file names</returns>
    Task<Result<IDictionary<string, string>>> GenerateDownloadUrlsBatchAsync(IEnumerable<string> fileNames, string? bucketName = null, int expirationMinutes = 60, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a media file from cloud storage
    /// </summary>
    /// <param name="fileName">Name of the file to delete</param>
    /// <param name="bucketName">Storage bucket name (optional, uses default if not specified)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> DeleteAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple media files from cloud storage
    /// </summary>
    /// <param name="fileNames">Collection of file names to delete</param>
    /// <param name="bucketName">Storage bucket name (optional, uses default if not specified)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> DeleteBatchAsync(IEnumerable<string> fileNames, string? bucketName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a media file exists in cloud storage
    /// </summary>
    /// <param name="fileName">Name of the file to check</param>
    /// <param name="bucketName">Storage bucket name (optional, uses default if not specified)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file exists, false otherwise</returns>
    Task<Result<bool>> ExistsAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metadata for a media file in cloud storage
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="bucketName">Storage bucket name (optional, uses default if not specified)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File metadata</returns>
    Task<Result<MediaFileMetadata>> GetMetadataAsync(string fileName, string? bucketName = null, CancellationToken cancellationToken = default);
}
