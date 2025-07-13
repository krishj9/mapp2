using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Application.Common.Models;

namespace MAPP.BuildingBlocks.Infrastructure.Storage;

/// <summary>
/// Extension methods for registering media storage services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Google Cloud Storage services to the service collection
    /// </summary>
    public static IServiceCollection AddGoogleCloudStorage(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configure storage options
        services.Configure<MediaStorageOptions>(options =>
            configuration.GetSection(MediaStorageOptions.SectionName).Bind(options));

        // Register Google Cloud Storage client
        services.AddSingleton<StorageClient>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<StorageClient>>();
            
            try
            {
                // Try to create the client - this will use default credentials or service account
                var client = StorageClient.Create();
                logger.LogInformation("Successfully initialized Google Cloud Storage client");
                return client;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize Google Cloud Storage client. Falling back to local storage.");
                throw;
            }
        });

        // Register storage service
        services.AddScoped<IMediaStorageService, GoogleCloudStorageService>();

        return services;
    }

    /// <summary>
    /// Adds media storage services for a specific environment
    /// </summary>
    public static IServiceCollection AddMediaStorageForEnvironment(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Configure storage options
        services.Configure<MediaStorageOptions>(options =>
            configuration.GetSection(MediaStorageOptions.SectionName).Bind(options));

        if (environment.IsProduction() || environment.IsStaging() || environment.EnvironmentName == "CloudStorageTest")
        {
            // Use Google Cloud Storage in production/staging/cloudstoragetest
            try
            {
                services.AddGoogleCloudStorage(configuration);
            }
            catch
            {
                // Fallback to local storage if GCS initialization fails
                services.AddLocalFileStorage(configuration);
            }
        }
        else
        {
            // Use local file storage for development
            services.AddLocalFileStorage(configuration);
        }

        return services;
    }

    /// <summary>
    /// Adds local file storage for development
    /// </summary>
    public static IServiceCollection AddLocalFileStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure storage options
        services.Configure<MediaStorageOptions>(options =>
        {
            configuration.GetSection(MediaStorageOptions.SectionName).Bind(options);
            
            // Set development defaults if not configured
            if (string.IsNullOrEmpty(options.DefaultBucketName))
            {
                options.DefaultBucketName = "local-dev-bucket";
            }
            
            if (string.IsNullOrEmpty(options.ProjectId))
            {
                options.ProjectId = "local-dev-project";
            }
        });

        // Register local file storage service
        services.AddScoped<IMediaStorageService, LocalFileStorageService>();

        return services;
    }

    /// <summary>
    /// Adds media storage with custom configuration
    /// </summary>
    public static IServiceCollection AddMediaStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MediaStorageOptions>? configureOptions = null)
    {
        // Configure storage options
        services.Configure<MediaStorageOptions>(options =>
        {
            configuration.GetSection(MediaStorageOptions.SectionName).Bind(options);
            configureOptions?.Invoke(options);
        });

        // Determine which storage implementation to use based on configuration
        var storageOptions = new MediaStorageOptions();
        configuration.GetSection(MediaStorageOptions.SectionName).Bind(storageOptions);
        configureOptions?.Invoke(storageOptions);

        if (!string.IsNullOrEmpty(storageOptions.ProjectId) && storageOptions.ProjectId != "local-dev-project")
        {
            // Use Google Cloud Storage
            try
            {
                services.AddSingleton<StorageClient>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<StorageClient>>();
                    
                    try
                    {
                        var client = StorageClient.Create();
                        logger.LogInformation("Successfully initialized Google Cloud Storage client for project {ProjectId}", storageOptions.ProjectId);
                        return client;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to initialize Google Cloud Storage client");
                        throw;
                    }
                });

                services.AddScoped<IMediaStorageService, GoogleCloudStorageService>();
            }
            catch
            {
                // Fallback to local storage
                services.AddScoped<IMediaStorageService, LocalFileStorageService>();
            }
        }
        else
        {
            // Use local file storage
            services.AddScoped<IMediaStorageService, LocalFileStorageService>();
        }

        return services;
    }
}

/// <summary>
/// Extension methods for validating media storage configuration
/// </summary>
public static class MediaStorageValidationExtensions
{
    /// <summary>
    /// Validates media storage configuration and logs warnings for missing settings
    /// </summary>
    public static IServiceCollection ValidateMediaStorageConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger logger)
    {
        var options = new MediaStorageOptions();
        configuration.GetSection(MediaStorageOptions.SectionName).Bind(options);

        var warnings = new List<string>();

        if (string.IsNullOrEmpty(options.DefaultBucketName))
        {
            warnings.Add("MediaStorage:DefaultBucketName is not configured");
        }

        if (string.IsNullOrEmpty(options.ProjectId))
        {
            warnings.Add("MediaStorage:ProjectId is not configured");
        }

        if (options.MaxFileSizeBytes <= 0)
        {
            warnings.Add("MediaStorage:MaxFileSizeBytes should be greater than 0");
        }

        if (options.AllowedFileExtensions?.Length == 0)
        {
            warnings.Add("MediaStorage:AllowedFileExtensions is empty - no file uploads will be allowed");
        }

        if (options.AllowedMimeTypes?.Length == 0)
        {
            warnings.Add("MediaStorage:AllowedMimeTypes is empty - no file uploads will be allowed");
        }

        foreach (var warning in warnings)
        {
            logger.LogWarning("[MEDIA STORAGE CONFIG] {Warning}", warning);
        }

        if (warnings.Any())
        {
            logger.LogInformation("[MEDIA STORAGE CONFIG] Found {WarningCount} configuration warnings. Using default values where applicable.", warnings.Count);
        }
        else
        {
            logger.LogInformation("[MEDIA STORAGE CONFIG] Configuration validation passed");
        }

        return services;
    }
}
