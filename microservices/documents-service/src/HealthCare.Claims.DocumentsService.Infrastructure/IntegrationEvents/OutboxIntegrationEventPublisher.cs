using HealthCare.Claims.DocumentsService.Application.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Outbox;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents
{
    public sealed class OutboxIntegrationEventPublisher(
        IOutboxStore outboxStore,
        ILogger<OutboxIntegrationEventPublisher> logger) : IIntegrationEventPublisher
    {
        private static readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);             
        public Task PublishDocumentRegisteredAsync(DocumentRegisteredIntegrationEvent integrationEvent, CancellationToken cancellationToken)
         => EnqueueAsync(nameof(DocumentRegisteredIntegrationEvent), integrationEvent.Id, integrationEvent.OccurredOn, integrationEvent, cancellationToken);

        public Task PublishDocumentRejectedAsync(DocumentRejectedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        => EnqueueAsync(nameof(DocumentRejectedIntegrationEvent), integrationEvent.Id, integrationEvent.OccurredOn, integrationEvent, cancellationToken);

        public Task PublishDocumentVerifiedAsync(DocumentVerifiedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        => EnqueueAsync(nameof(DocumentVerifiedIntegrationEvent), integrationEvent.Id, integrationEvent.OccurredOn, integrationEvent, cancellationToken);

        private async Task EnqueueAsync<TEvent>(
            string eventType,
            Guid eventId,
            DateTimeOffset occurredOn,
            TEvent integrationEvent,
            CancellationToken cancellationToken)
        {
            var paload = JsonSerializer.Serialize(integrationEvent, jsonOptions);
            var message = new OutboxMessage(eventId, eventType, occurredOn, paload);

            await outboxStore.AddAsync(message, cancellationToken);
            logger.LogInformation("Queued {EventType} in the Documents outbox as {MessageId}", eventType, message.Id);
        }
    }
}
