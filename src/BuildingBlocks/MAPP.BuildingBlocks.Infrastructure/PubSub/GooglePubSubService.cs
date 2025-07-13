using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using MAPP.BuildingBlocks.Domain.Events;
using Google.Apis.Auth.OAuth2;

namespace MAPP.BuildingBlocks.Infrastructure.PubSub;

/// <summary>
/// Google Cloud Pub/Sub implementation for domain event publishing and consuming
/// </summary>
public class GooglePubSubService : IPubSubService, IDisposable
{
    private readonly ILogger<GooglePubSubService> _logger;
    private readonly PubSubOptions _options;
    private readonly Dictionary<string, PublisherClient> _publishers = new();
    private readonly Dictionary<string, SubscriberClient> _subscribers = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public GooglePubSubService(
        ILogger<GooglePubSubService> logger,
        IOptions<PubSubOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        // Publisher will be created per topic in PublishAsync method
    }

    public async Task PublishAsync<T>(T domainEvent, string topicName, CancellationToken cancellationToken = default)
        where T : BaseEvent
    {
        try
        {
            var topicPath = TopicName.FromProjectTopic(_options.ProjectId, topicName);

            // Get or create publisher for this topic
            if (!_publishers.TryGetValue(topicName, out var publisher))
            {
                publisher = await PublisherClient.CreateAsync(topicPath);
                _publishers[topicName] = publisher;
            }

            var message = new PubsubMessage
            {
                Data = Google.Protobuf.ByteString.CopyFromUtf8(JsonSerializer.Serialize(domainEvent)),
                Attributes =
                {
                    ["eventType"] = typeof(T).Name,
                    ["eventId"] = Guid.NewGuid().ToString(),
                    ["timestamp"] = domainEvent.DateOccurred.ToString("O"),
                    ["source"] = _options.ServiceName
                }
            };

            var messageId = await publisher.PublishAsync(message);

            _logger.LogInformation(
                "Published domain event {EventType} to topic {TopicName} with message ID {MessageId}",
                typeof(T).Name, topicName, messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to publish domain event {EventType} to topic {TopicName}",
                typeof(T).Name, topicName);
            throw;
        }
    }

    public async Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default)
        where T : BaseEvent
    {
        var topicName = GetDefaultTopicName(domainEvent.GetType());
        await PublishAsync(domainEvent, topicName, cancellationToken);
    }

    public async Task SubscribeAsync<T>(string subscriptionName, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : BaseEvent
    {
        try
        {
            var subscriptionPath = SubscriptionName.FromProjectSubscription(_options.ProjectId, subscriptionName);
            var subscriber = await SubscriberClient.CreateAsync(subscriptionPath);

            _subscribers[subscriptionName] = subscriber;

            await subscriber.StartAsync(async (message, ct) =>
            {
                try
                {
                    var eventData = JsonSerializer.Deserialize<T>(message.Data.ToStringUtf8());
                    if (eventData != null)
                    {
                        await handler(eventData, ct);
                        return SubscriberClient.Reply.Ack;
                    }
                    
                    _logger.LogWarning("Failed to deserialize message for subscription {SubscriptionName}", subscriptionName);
                    return SubscriberClient.Reply.Nack;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message in subscription {SubscriptionName}", subscriptionName);
                    return SubscriberClient.Reply.Nack;
                }
            });

            _logger.LogInformation("Started subscription {SubscriptionName} for event type {EventType}", 
                subscriptionName, typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start subscription {SubscriptionName}", subscriptionName);
            throw;
        }
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Pub/Sub message consumption for {SubscriptionCount} subscriptions", 
            _subscribers.Count);
        
        // Subscriptions are already started in SubscribeAsync
        await Task.CompletedTask;
    }

    public async Task StopConsumingAsync()
    {
        _logger.LogInformation("Stopping Pub/Sub message consumption");
        
        foreach (var subscriber in _subscribers.Values)
        {
            await subscriber.StopAsync(CancellationToken.None);
        }
        
        _subscribers.Clear();
        _cancellationTokenSource.Cancel();
    }

    private string GetDefaultTopicName<T>() where T : BaseEvent
    {
        return GetDefaultTopicName(typeof(T));
    }

    private string GetDefaultTopicName(Type eventType)
    {
        var eventTypeName = eventType.Name;

        // Check if there's a specific mapping for this event type
        if (_options.TopicMappings.TryGetValue(eventTypeName, out var mappedTopicName))
        {
            return mappedTopicName;
        }

        // Convert event name to lowercase and remove "Event" suffix
        var topicName = eventTypeName.ToLowerInvariant();
        if (topicName.EndsWith("event"))
        {
            topicName = topicName.Substring(0, topicName.Length - 5);
        }

        return topicName;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.StopAsync(CancellationToken.None).Wait(TimeSpan.FromSeconds(5));
        }

        foreach (var publisher in _publishers.Values)
        {
            publisher.ShutdownAsync(TimeSpan.FromSeconds(5)).Wait();
        }

        _subscribers.Clear();
        _publishers.Clear();

        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Configuration options for Google Pub/Sub
/// </summary>
public class PubSubOptions
{
    public const string SectionName = "PubSub";
    
    public string ProjectId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string TopicPrefix { get; set; } = "mapp";
    public bool EnablePublishing { get; set; } = true;
    public bool EnableConsuming { get; set; } = true;
    public Dictionary<string, string> TopicMappings { get; set; } = new();
    public Dictionary<string, string> SubscriptionMappings { get; set; } = new();
}
