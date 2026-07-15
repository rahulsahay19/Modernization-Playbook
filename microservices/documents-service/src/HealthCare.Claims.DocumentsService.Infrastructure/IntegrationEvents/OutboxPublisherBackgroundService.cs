using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public sealed class OutboxPublisherBackgroundService
        (IOutboxStore outboxStore,
        IIntegrationEventBroker broker,
        IOptions<IntegrationEventBrokerOptions> options,
        ILogger<OutboxPublisherBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromSeconds(Math.Max(1, options.Value.PublishIntervalSeconds));

            while(!stoppingToken.IsCancellationRequested)
            {
                await PublishPendingMessagesAsync(stoppingToken);
                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
        {
            var pendingMessages = await outboxStore.GetPendingAsync(batchSize: 20, cancellationToken);
            foreach (var message in pendingMessages)
            {
                try 
                {
                    await broker.PublishAsync(message, cancellationToken);
                    await outboxStore.MarkPublishedAsync(message.Id, DateTimeOffset.UtcNow, cancellationToken);
                    logger.LogInformation("Piblished outbox message {MessageId} to the integration-event broker", message.Id);
                }
                catch(Exception ex) when (ex is not OperationCanceledException)
                {
                    await outboxStore.MarkFailedAsync(message.Id, ex.Message, cancellationToken);
                    logger.LogInformation(ex, "Could not publish outbox message {MessageId}; it will be retried", message.Id);
                }
            }
        }
    }
}
