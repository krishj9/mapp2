using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.Observations.Domain.Events;
using MAPP.Modules.Observations.Domain.ValueObjects;
using MAPP.Modules.Observations.Domain.Enums;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Observation artifact entity for observations
/// </summary>
public class ObservationArtifact : BaseAuditableEntity
{
    public long ObservationId { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public string StoredFileName { get; private set; } = string.Empty;
    public string BucketName { get; private set; } = string.Empty;
    public FilePath StoragePath { get; private set; } = null!;
    public string? PublicUrl { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public FileSize FileSizeBytes { get; private set; } = null!;
    public MediaType MediaType { get; private set; }
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsUploaded { get; private set; }
    public string? UploadError { get; private set; }
    public DateTime? UploadedDate { get; private set; }
    public string? UploadedBy { get; private set; }
    public string? Metadata { get; private set; }

    // Navigation property
    public Observation Observation { get; private set; } = null!;

    // Private constructor for EF Core
    private ObservationArtifact() { }

    public ObservationArtifact(
        long observationId,
        string originalFileName,
        string contentType,
        long fileSizeBytes,
        string? caption = null,
        int displayOrder = 0,
        string? metadata = null)
    {
        ObservationId = Guard.Against.NegativeOrZero(observationId, nameof(observationId));
        OriginalFileName = Guard.Against.NullOrEmpty(originalFileName, nameof(originalFileName));
        ContentType = Guard.Against.NullOrEmpty(contentType, nameof(contentType));
        FileSizeBytes = FileSize.FromBytes(fileSizeBytes);
        Caption = caption;
        DisplayOrder = displayOrder;
        Metadata = metadata;
        IsUploaded = false;
        
        MediaType = DetermineMediaType(contentType);
        StoredFileName = GenerateStoredFileName(originalFileName);
        StoragePath = new FilePath(GenerateStoragePath());
    }

    public void SetStorageDetails(string bucketName, string storagePath, string? publicUrl = null)
    {
        BucketName = Guard.Against.NullOrEmpty(bucketName, nameof(bucketName));
        StoragePath = new FilePath(storagePath);
        PublicUrl = publicUrl;
    }

    public void MarkAsUploaded(string uploadedBy)
    {
        IsUploaded = true;
        UploadedDate = DateTime.UtcNow;
        UploadedBy = Guard.Against.NullOrEmpty(uploadedBy, nameof(uploadedBy));
        UploadError = null;
        
        AddDomainEvent(new ObservationArtifactUploadedEvent(this));
    }

    public void SetUploadError(string error)
    {
        IsUploaded = false;
        UploadError = Guard.Against.NullOrEmpty(error, nameof(error));
        
        AddDomainEvent(new ObservationArtifactUploadFailedEvent(this, error));
    }

    public void UpdateCaption(string? caption)
    {
        Caption = caption;
    }

    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = Guard.Against.Negative(displayOrder, nameof(displayOrder));
    }

    public void UpdateMetadata(string? metadata)
    {
        Metadata = metadata;
    }

    public string GetFileExtension()
    {
        return Path.GetExtension(OriginalFileName);
    }

    private static MediaType DetermineMediaType(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            var ct when ct.StartsWith("image/") => MediaType.Image,
            var ct when ct.StartsWith("video/") => MediaType.Video,
            var ct when ct.StartsWith("audio/") => MediaType.Audio,
            "application/pdf" => MediaType.Document,
            var ct when ct.StartsWith("application/") => MediaType.Document,
            var ct when ct.StartsWith("text/") => MediaType.Document,
            _ => MediaType.Other
        };
    }

    private string GenerateStoredFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var uniqueId = Guid.NewGuid().ToString("N");
        return $"{uniqueId}{extension}";
    }

    private string GenerateStoragePath()
    {
        var dateFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
        return $"observations/{ObservationId}/media/{dateFolder}";
    }
} 