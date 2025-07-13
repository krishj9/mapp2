namespace MAPP.BuildingBlocks.Application.Common.Models;

/// <summary>
/// Request model for uploading media files
/// </summary>
public class MediaUploadRequest
{
    /// <summary>
    /// File content as byte array
    /// </summary>
    public required byte[] FileContent { get; init; }

    /// <summary>
    /// Original file name
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// MIME content type of the file
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Target bucket name (optional, uses default if not specified)
    /// </summary>
    public string? BucketName { get; init; }

    /// <summary>
    /// Custom folder/prefix path within the bucket
    /// </summary>
    public string? FolderPath { get; init; }

    /// <summary>
    /// Whether to generate a unique file name to prevent conflicts
    /// </summary>
    public bool GenerateUniqueFileName { get; init; } = true;

    /// <summary>
    /// Custom metadata to associate with the file
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Cache control header for the file
    /// </summary>
    public string? CacheControl { get; init; }

    /// <summary>
    /// Whether the file should be publicly accessible
    /// </summary>
    public bool IsPublic { get; init; } = false;
}

/// <summary>
/// Result model for media file uploads
/// </summary>
public class MediaUploadResult
{
    /// <summary>
    /// Unique identifier for the uploaded file
    /// </summary>
    public required string FileId { get; init; }

    /// <summary>
    /// Final file name in storage (may be different from original if unique name was generated)
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Original file name as provided in the request
    /// </summary>
    public required string OriginalFileName { get; init; }

    /// <summary>
    /// Bucket name where the file was stored
    /// </summary>
    public required string BucketName { get; init; }

    /// <summary>
    /// Full path/key of the file in storage
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// Public URL of the file (if file is public)
    /// </summary>
    public string? PublicUrl { get; init; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; init; }

    /// <summary>
    /// MIME content type
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Upload timestamp
    /// </summary>
    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ETag or version identifier from storage
    /// </summary>
    public string? ETag { get; init; }

    /// <summary>
    /// Custom metadata associated with the file
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Metadata information for a media file
/// </summary>
public record MediaFileMetadata
{
    /// <summary>
    /// File name in storage
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Bucket name where the file is stored
    /// </summary>
    public required string BucketName { get; init; }

    /// <summary>
    /// Full path/key of the file in storage
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; init; }

    /// <summary>
    /// MIME content type
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// File creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// File last modified timestamp
    /// </summary>
    public DateTime LastModified { get; init; }

    /// <summary>
    /// ETag or version identifier from storage
    /// </summary>
    public string? ETag { get; init; }

    /// <summary>
    /// MD5 hash of the file content
    /// </summary>
    public string? MD5Hash { get; init; }

    /// <summary>
    /// Custom metadata associated with the file
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Whether the file is publicly accessible
    /// </summary>
    public bool IsPublic { get; init; }

    /// <summary>
    /// Cache control header
    /// </summary>
    public string? CacheControl { get; init; }
}

/// <summary>
/// Configuration options for media storage
/// </summary>
public class MediaStorageOptions
{
    public const string SectionName = "MediaStorage";

    /// <summary>
    /// Default bucket name for media files
    /// </summary>
    public string DefaultBucketName { get; set; } = string.Empty;

    /// <summary>
    /// GCP project ID
    /// </summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// Default expiration time for signed URLs in minutes
    /// </summary>
    public int DefaultSignedUrlExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Maximum file size allowed in bytes
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024; // 100MB

    /// <summary>
    /// Allowed file extensions
    /// </summary>
    public string[] AllowedFileExtensions { get; set; } = 
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", // Images
        ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm", // Videos
        ".mp3", ".wav", ".flac", ".aac", ".ogg", // Audio
        ".pdf", ".doc", ".docx", ".txt", ".rtf" // Documents
    };

    /// <summary>
    /// Allowed MIME types
    /// </summary>
    public string[] AllowedMimeTypes { get; set; } = 
    {
        "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp",
        "video/mp4", "video/avi", "video/quicktime", "video/x-ms-wmv", "video/x-flv", "video/webm",
        "audio/mpeg", "audio/wav", "audio/flac", "audio/aac", "audio/ogg",
        "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "text/plain", "application/rtf"
    };

    /// <summary>
    /// Default cache control header for uploaded files
    /// </summary>
    public string DefaultCacheControl { get; set; } = "public, max-age=3600";

    /// <summary>
    /// Whether to enable automatic file compression
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Default folder structure for organizing files
    /// </summary>
    public string DefaultFolderStructure { get; set; } = "{domain}/{year}/{month}";
}
