using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MAPP.BuildingBlocks.Infrastructure.PubSub;

/// <summary>
/// Extension methods for registering Pub/Sub services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Google Pub/Sub services to the service collection
    /// </summary>
    public static IServiceCollection AddGooglePubSub(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configure Pub/Sub options
        services.Configure<PubSubOptions>(options =>
            configuration.GetSection(PubSubOptions.SectionName).Bind(options));

        // Register Pub/Sub service
        services.AddSingleton<IPubSubService, GooglePubSubService>();

        // Register external event consumer as hosted service
        services.AddHostedService<ExternalEventConsumerService>();

        return services;
    }

    /// <summary>
    /// Adds Pub/Sub services for a specific environment
    /// </summary>
    public static IServiceCollection AddPubSubForEnvironment(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (environment.IsProduction() || environment.IsStaging() || environment.EnvironmentName == "PubSubTest")
        {
            // Use Google Pub/Sub in production/staging/pubsubtest
            services.AddGooglePubSub(configuration);
        }
        else
        {
            // Use in-memory or local emulator for development
            services.AddLocalPubSub(configuration);
        }

        return services;
    }

    /// <summary>
    /// Adds local/in-memory Pub/Sub for development
    /// </summary>
    public static IServiceCollection AddLocalPubSub(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Pub/Sub options
        services.Configure<PubSubOptions>(options =>
        {
            options.EnablePublishing = false; // Disable external publishing in dev
            options.EnableConsuming = false;  // Disable external consuming in dev
        });

        // Register a no-op Pub/Sub service for development
        services.AddSingleton<IPubSubService, LocalPubSubService>();

        return services;
    }
}

/// <summary>
/// Local/development implementation of Pub/Sub service
/// Logs events instead of actually publishing them
/// </summary>
public class LocalPubSubService : IPubSubService
{
    private readonly ILogger<LocalPubSubService> _logger;

    public LocalPubSubService(ILogger<LocalPubSubService> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T domainEvent, string topicName, CancellationToken cancellationToken = default)
        where T : MAPP.BuildingBlocks.Domain.Events.BaseEvent
    {
        _logger.LogInformation(
            "[LOCAL PUB/SUB] Would publish {EventType} to GCP topic: projects/mapp-dev-457512/topics/{TopicName}",
            typeof(T).Name, topicName);

        _logger.LogInformation(
            "[LOCAL PUB/SUB] Event payload: {Event}",
            System.Text.Json.JsonSerializer.Serialize(domainEvent));

        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default)
        where T : MAPP.BuildingBlocks.Domain.Events.BaseEvent
    {
        // Simulate the same topic name logic as the real service
        var eventTypeName = typeof(T).Name;
        var topicName = eventTypeName.ToLowerInvariant();
        if (topicName.EndsWith("event"))
        {
            topicName = topicName.Substring(0, topicName.Length - 5);
        }

        return PublishAsync(domainEvent, topicName, cancellationToken);
    }

    public Task SubscribeAsync<T>(string subscriptionName, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : MAPP.BuildingBlocks.Domain.Events.BaseEvent
    {
        _logger.LogInformation(
            "[LOCAL PUB/SUB] Would subscribe to {SubscriptionName} for event type {EventType}",
            subscriptionName, typeof(T).Name);
        
        return Task.CompletedTask;
    }

    public Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[LOCAL PUB/SUB] Would start consuming messages");
        return Task.CompletedTask;
    }

    public Task StopConsumingAsync()
    {
        _logger.LogInformation("[LOCAL PUB/SUB] Would stop consuming messages");
        return Task.CompletedTask;
    }
}
