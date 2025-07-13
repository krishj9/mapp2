using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MAPP.BuildingBlocks.Domain.Events;

namespace MAPP.BuildingBlocks.Infrastructure.PubSub;

/// <summary>
/// Background service that consumes external domain events from Pub/Sub
/// and publishes them locally via MediatR for processing
/// </summary>
public class ExternalEventConsumerService : BackgroundService
{
    private readonly ILogger<ExternalEventConsumerService> _logger;
    private readonly IPubSubService _pubSubService;
    private readonly IServiceProvider _serviceProvider;
    private readonly PubSubOptions _options;

    public ExternalEventConsumerService(
        ILogger<ExternalEventConsumerService> logger,
        IPubSubService pubSubService,
        IServiceProvider serviceProvider,
        IOptions<PubSubOptions> options)
    {
        _logger = logger;
        _pubSubService = pubSubService;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.EnableConsuming)
        {
            _logger.LogInformation("External event consumption is disabled");
            return;
        }

        _logger.LogInformation("Starting external event consumer service");

        try
        {
            // Subscribe to external observation events
            await SetupObservationEventSubscriptions(stoppingToken);
            
            // Subscribe to external planning events
            await SetupPlanningEventSubscriptions(stoppingToken);
            
            // Add more domain subscriptions as needed
            
            // Start consuming messages
            await _pubSubService.StartConsumingAsync(stoppingToken);
            
            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("External event consumer service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External event consumer service encountered an error");
            throw;
        }
    }

    private async Task SetupObservationEventSubscriptions(CancellationToken cancellationToken)
    {
        // Note: Specific event subscriptions should be configured in the respective service startup
        // This is a generic consumer service that can be extended per domain
        _logger.LogInformation("External event consumer service ready for domain-specific subscriptions");
        await Task.CompletedTask;
    }

    private async Task SetupPlanningEventSubscriptions(CancellationToken cancellationToken)
    {
        // Subscribe to external planning events when needed
        // await _pubSubService.SubscribeAsync<ExternalPlanCreatedEvent>(
        //     "mapp-plans-created-subscription",
        //     HandleExternalPlanCreatedEvent,
        //     cancellationToken);

        await Task.CompletedTask;
    }

    // Domain-specific event handlers should be implemented in the respective services
    // This base service provides the infrastructure for consuming external events

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping external event consumer service");
        await _pubSubService.StopConsumingAsync();
        await base.StopAsync(cancellationToken);
    }
}

// External event classes are now defined in the respective Application layers
// to maintain Clean Architecture principles
