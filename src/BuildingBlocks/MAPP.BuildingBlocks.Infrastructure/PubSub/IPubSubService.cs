using MAPP.BuildingBlocks.Domain.Events;

namespace MAPP.BuildingBlocks.Infrastructure.PubSub;

/// <summary>
/// Interface for Pub/Sub service to publish and consume domain events
/// Supports Google Cloud Pub/Sub and other message brokers
/// </summary>
public interface IPubSubService
{
    /// <summary>
    /// Publishes a domain event to the specified topic
    /// </summary>
    Task PublishAsync<T>(T domainEvent, string topicName, CancellationToken cancellationToken = default) 
        where T : BaseEvent;

    /// <summary>
    /// Publishes a domain event to the default topic for the event type
    /// </summary>
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) 
        where T : BaseEvent;

    /// <summary>
    /// Subscribes to a topic and handles incoming messages
    /// </summary>
    Task SubscribeAsync<T>(string subscriptionName, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) 
        where T : BaseEvent;

    /// <summary>
    /// Starts consuming messages from all configured subscriptions
    /// </summary>
    Task StartConsumingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops consuming messages
    /// </summary>
    Task StopConsumingAsync();
}
