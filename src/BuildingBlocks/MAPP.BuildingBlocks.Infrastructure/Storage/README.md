# MAPP Media Storage Service

This module provides a comprehensive media storage solution for the MAPP system, supporting both Google Cloud Storage (production) and local file storage (development).

## Features

- ✅ **File Upload**: Single and batch file uploads
- ✅ **Signed URLs**: Generate secure download URLs with expiration
- ✅ **File Management**: Delete, check existence, get metadata
- ✅ **Validation**: File type, size, and MIME type validation
- ✅ **Environment Support**: GCS for production, local storage for development
- ✅ **Flexible Configuration**: Customizable buckets, folders, and file naming
- ✅ **Web API Endpoints**: Ready-to-use FastEndpoints for file operations

## Quick Start

### 1. Configuration

Add to your `appsettings.json`:

```json
{
  "MediaStorage": {
    "DefaultBucketName": "mapp-media-files",
    "ProjectId": "mapp-dev-457512",
    "DefaultSignedUrlExpirationMinutes": 60,
    "MaxFileSizeBytes": 104857600,
    "DefaultFolderStructure": "{domain}/{year}/{month}"
  }
}
```

### 2. Service Registration

In your `Program.cs` or service configuration:

```csharp
// Option 1: Environment-based (recommended)
services.AddMediaStorageForEnvironment(configuration, environment);

// Option 2: Explicit Google Cloud Storage
services.AddGoogleCloudStorage(configuration);

// Option 3: Local file storage only
services.AddLocalFileStorage(configuration);

// Option 4: Custom configuration
services.AddMediaStorage(configuration, options =>
{
    options.DefaultBucketName = "custom-bucket";
    options.MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB
});
```

### 3. Web API Integration

```csharp
// Add web services with media storage endpoints
services.AddWebServicesWithMediaStorage(configuration, environment);

// In your app configuration
app.UseFastEndpoints();
```

## Usage Examples

### Programmatic Usage

```csharp
public class MyService
{
    private readonly IMediaStorageService _storageService;

    public MyService(IMediaStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<string> UploadImageAsync(byte[] imageData, string fileName)
    {
        var request = new MediaUploadRequest
        {
            FileContent = imageData,
            FileName = fileName,
            ContentType = "image/jpeg",
            FolderPath = "observations/images",
            GenerateUniqueFileName = true,
            IsPublic = false,
            Metadata = new Dictionary<string, string>
            {
                ["uploadedBy"] = "user123",
                ["category"] = "observation"
            }
        };

        var result = await _storageService.UploadAsync(request);
        
        if (result.IsSuccess)
        {
            return result.Value.FilePath;
        }
        
        throw new Exception($"Upload failed: {string.Join(", ", result.Errors)}");
    }

    public async Task<string> GetDownloadUrlAsync(string fileName)
    {
        var result = await _storageService.GenerateDownloadUrlAsync(
            fileName, 
            expirationMinutes: 30);
            
        return result.IsSuccess ? result.Value : throw new Exception("Failed to generate URL");
    }
}
```

### Web API Usage

#### Upload Single File
```http
POST /api/media/upload
Content-Type: multipart/form-data

{
  "file": [binary data],
  "folderPath": "observations/images",
  "generateUniqueFileName": true,
  "isPublic": false
}
```

#### Upload Multiple Files
```http
POST /api/media/upload/batch
Content-Type: multipart/form-data

{
  "files": [binary data array],
  "folderPath": "observations/documents",
  "generateUniqueFileName": true
}
```

#### Generate Download URL
```http
POST /api/media/download-url
Content-Type: application/json

{
  "fileName": "observations/images/photo_20241213_123456_abc12345.jpg",
  "expirationMinutes": 60
}
```

#### Generate Batch Download URLs
```http
POST /api/media/download-urls
Content-Type: application/json

{
  "fileNames": [
    "observations/images/photo1.jpg",
    "observations/images/photo2.jpg"
  ],
  "expirationMinutes": 30
}
```

#### Delete File
```http
DELETE /api/media/delete
Content-Type: application/json

{
  "fileName": "observations/images/photo_to_delete.jpg"
}
```

## Configuration Options

| Option | Description | Default |
|--------|-------------|---------|
| `DefaultBucketName` | Default storage bucket | `""` |
| `ProjectId` | GCP project ID | `""` |
| `DefaultSignedUrlExpirationMinutes` | URL expiration time | `60` |
| `MaxFileSizeBytes` | Maximum file size | `104857600` (100MB) |
| `AllowedFileExtensions` | Permitted file extensions | Images, videos, audio, docs |
| `AllowedMimeTypes` | Permitted MIME types | Common media types |
| `DefaultCacheControl` | Cache control header | `"public, max-age=3600"` |
| `EnableCompression` | Enable file compression | `true` |
| `DefaultFolderStructure` | Folder organization pattern | `"{domain}/{year}/{month}"` |

## Folder Structure Variables

The `DefaultFolderStructure` supports these variables:
- `{domain}` - Domain name (e.g., "observations", "planning")
- `{year}` - Current year (e.g., "2024")
- `{month}` - Current month with leading zero (e.g., "03")

Example: `"{domain}/{year}/{month}"` → `"observations/2024/12"`

## Environment Behavior

- **Development**: Uses local file storage in `./LocalStorage/` directory
- **Production/Staging**: Uses Google Cloud Storage
- **CloudStorageTest**: Forces GCS usage for testing

## Security Considerations

1. **File Validation**: All uploads are validated for type, size, and content
2. **Signed URLs**: Download URLs are time-limited and secure
3. **Access Control**: Files are private by default
4. **Metadata**: Custom metadata can be attached to files
5. **Error Handling**: Comprehensive error reporting without exposing internals

## Troubleshooting

### Common Issues

1. **GCS Authentication**: Ensure service account credentials are properly configured
2. **Bucket Permissions**: Verify bucket exists and service account has access
3. **File Size Limits**: Check `MaxFileSizeBytes` configuration
4. **MIME Type Errors**: Ensure file types are in `AllowedMimeTypes`

### Logging

The service provides detailed logging at different levels:
- `Information`: Successful operations
- `Warning`: Configuration issues
- `Error`: Operation failures
- `Debug`: Detailed operation traces

## Integration with Domains

### Observations Domain
```csharp
// In ObservationsModule DI
services.AddMediaStorageForEnvironment(configuration, environment);

// Usage in observation service
var photoUpload = await _storageService.UploadAsync(new MediaUploadRequest
{
    FileContent = photoData,
    FileName = "observation_photo.jpg",
    ContentType = "image/jpeg",
    FolderPath = "observations/photos",
    Metadata = new Dictionary<string, string>
    {
        ["observationId"] = observationId.ToString(),
        ["capturedAt"] = DateTime.UtcNow.ToString("O")
    }
});
```

### Planning Domain
```csharp
// Similar integration for planning documents and media
var documentUpload = await _storageService.UploadAsync(new MediaUploadRequest
{
    FileContent = documentData,
    FileName = "plan_document.pdf",
    ContentType = "application/pdf",
    FolderPath = "planning/documents",
    Metadata = new Dictionary<string, string>
    {
        ["planId"] = planId.ToString(),
        ["version"] = "1.0"
    }
});
```

This media storage solution provides a robust, scalable foundation for handling file uploads and downloads across all MAPP domains.
