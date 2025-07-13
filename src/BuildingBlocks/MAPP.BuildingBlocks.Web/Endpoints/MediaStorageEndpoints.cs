using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Application.Common.Models;

namespace MAPP.BuildingBlocks.Web.Endpoints;

/// <summary>
/// Request model for single file upload
/// </summary>
public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public string? BucketName { get; set; }
    public string? FolderPath { get; set; }
    public bool GenerateUniqueFileName { get; set; } = true;
    public bool IsPublic { get; set; } = false;
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Request model for batch file upload
/// </summary>
public class UploadFilesRequest
{
    public IFormFileCollection Files { get; set; } = null!;
    public string? BucketName { get; set; }
    public string? FolderPath { get; set; }
    public bool GenerateUniqueFileName { get; set; } = true;
    public bool IsPublic { get; set; } = false;
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Request model for generating download URLs
/// </summary>
public class GenerateDownloadUrlRequest
{
    public required string FileName { get; set; }
    public string? BucketName { get; set; }
    public int ExpirationMinutes { get; set; } = 60;
}

/// <summary>
/// Request model for generating batch download URLs
/// </summary>
public class GenerateDownloadUrlsRequest
{
    public required string[] FileNames { get; set; }
    public string? BucketName { get; set; }
    public int ExpirationMinutes { get; set; } = 60;
}

/// <summary>
/// Request model for deleting files
/// </summary>
public class DeleteFileRequest
{
    public required string FileName { get; set; }
    public string? BucketName { get; set; }
}

/// <summary>
/// Request model for batch file deletion
/// </summary>
public class DeleteFilesRequest
{
    public required string[] FileNames { get; set; }
    public string? BucketName { get; set; }
}

/// <summary>
/// Endpoint for uploading a single media file
/// </summary>
public class UploadFileEndpoint : Endpoint<UploadFileRequest, MediaUploadResult>
{
    private readonly IMediaStorageService _storageService;
    private readonly ILogger<UploadFileEndpoint> _logger;

    public UploadFileEndpoint(IMediaStorageService storageService, ILogger<UploadFileEndpoint> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/media/upload");
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Upload a media file";
            s.Description = "Uploads a single media file to cloud storage and returns file metadata";
        });
    }

    public override async Task HandleAsync(UploadFileRequest req, CancellationToken ct)
    {
        if (req.File == null || req.File.Length == 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        try
        {
            using var stream = req.File.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, ct);

            var uploadRequest = new MediaUploadRequest
            {
                FileContent = memoryStream.ToArray(),
                FileName = req.File.FileName,
                ContentType = req.File.ContentType,
                BucketName = req.BucketName,
                FolderPath = req.FolderPath,
                GenerateUniqueFileName = req.GenerateUniqueFileName,
                IsPublic = req.IsPublic,
                Metadata = req.Metadata
            };

            var result = await _storageService.UploadAsync(uploadRequest, ct);

            if (result.Succeeded)
            {
                await SendOkAsync(result.Data!, ct);
            }
            else
            {
                _logger.LogError("Failed to upload file {FileName}: {Errors}", req.File.FileName, string.Join(", ", result.Errors));
                await SendErrorsAsync(400, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", req.File.FileName);
            await SendErrorsAsync(500, ct);
        }
    }
}

/// <summary>
/// Endpoint for uploading multiple media files
/// </summary>
public class UploadFilesEndpoint : Endpoint<UploadFilesRequest, IEnumerable<MediaUploadResult>>
{
    private readonly IMediaStorageService _storageService;
    private readonly ILogger<UploadFilesEndpoint> _logger;

    public UploadFilesEndpoint(IMediaStorageService storageService, ILogger<UploadFilesEndpoint> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/media/upload/batch");
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Upload multiple media files";
            s.Description = "Uploads multiple media files to cloud storage and returns file metadata for each";
        });
    }

    public override async Task HandleAsync(UploadFilesRequest req, CancellationToken ct)
    {
        if (req.Files == null || req.Files.Count == 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        try
        {
            var uploadRequests = new List<MediaUploadRequest>();

            foreach (var file in req.Files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream, ct);

                    uploadRequests.Add(new MediaUploadRequest
                    {
                        FileContent = memoryStream.ToArray(),
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        BucketName = req.BucketName,
                        FolderPath = req.FolderPath,
                        GenerateUniqueFileName = req.GenerateUniqueFileName,
                        IsPublic = req.IsPublic,
                        Metadata = req.Metadata
                    });
                }
            }

            var result = await _storageService.UploadBatchAsync(uploadRequests, ct);

            if (result.Succeeded)
            {
                await SendOkAsync(result.Data!, ct);
            }
            else
            {
                _logger.LogError("Failed to upload files: {Errors}", string.Join(", ", result.Errors));
                await SendErrorsAsync(400, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading files");
            await SendErrorsAsync(500, ct);
        }
    }
}

/// <summary>
/// Endpoint for generating download URL for a single file
/// </summary>
public class GenerateDownloadUrlEndpoint : Endpoint<GenerateDownloadUrlRequest, string>
{
    private readonly IMediaStorageService _storageService;
    private readonly ILogger<GenerateDownloadUrlEndpoint> _logger;

    public GenerateDownloadUrlEndpoint(IMediaStorageService storageService, ILogger<GenerateDownloadUrlEndpoint> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/media/download-url");
        Summary(s =>
        {
            s.Summary = "Generate download URL";
            s.Description = "Generates a signed URL for downloading a media file";
        });
    }

    public override async Task HandleAsync(GenerateDownloadUrlRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _storageService.GenerateDownloadUrlAsync(req.FileName, req.BucketName, req.ExpirationMinutes, ct);

            if (result.Succeeded)
            {
                await SendOkAsync(result.Data!, ct);
            }
            else
            {
                _logger.LogError("Failed to generate download URL for {FileName}: {Errors}", req.FileName, string.Join(", ", result.Errors));
                await SendErrorsAsync(404, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating download URL for {FileName}", req.FileName);
            await SendErrorsAsync(500, ct);
        }
    }
}

/// <summary>
/// Endpoint for generating download URLs for multiple files
/// </summary>
public class GenerateDownloadUrlsEndpoint : Endpoint<GenerateDownloadUrlsRequest, IDictionary<string, string>>
{
    private readonly IMediaStorageService _storageService;
    private readonly ILogger<GenerateDownloadUrlsEndpoint> _logger;

    public GenerateDownloadUrlsEndpoint(IMediaStorageService storageService, ILogger<GenerateDownloadUrlsEndpoint> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/media/download-urls");
        Summary(s =>
        {
            s.Summary = "Generate download URLs";
            s.Description = "Generates signed URLs for downloading multiple media files";
        });
    }

    public override async Task HandleAsync(GenerateDownloadUrlsRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _storageService.GenerateDownloadUrlsBatchAsync(req.FileNames, req.BucketName, req.ExpirationMinutes, ct);

            if (result.Succeeded)
            {
                await SendOkAsync(result.Data!, ct);
            }
            else
            {
                _logger.LogError("Failed to generate download URLs: {Errors}", string.Join(", ", result.Errors));
                await SendErrorsAsync(400, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating download URLs");
            await SendErrorsAsync(500, ct);
        }
    }
}

/// <summary>
/// Endpoint for deleting a media file
/// </summary>
public class DeleteFileEndpoint : Endpoint<DeleteFileRequest>
{
    private readonly IMediaStorageService _storageService;
    private readonly ILogger<DeleteFileEndpoint> _logger;

    public DeleteFileEndpoint(IMediaStorageService storageService, ILogger<DeleteFileEndpoint> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/api/media/delete");
        Summary(s =>
        {
            s.Summary = "Delete media file";
            s.Description = "Deletes a media file from cloud storage";
        });
    }

    public override async Task HandleAsync(DeleteFileRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _storageService.DeleteAsync(req.FileName, req.BucketName, ct);

            if (result.Succeeded)
            {
                await SendNoContentAsync(ct);
            }
            else
            {
                _logger.LogError("Failed to delete file {FileName}: {Errors}", req.FileName, string.Join(", ", result.Errors));
                await SendErrorsAsync(400, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName}", req.FileName);
            await SendErrorsAsync(500, ct);
        }
    }
}
