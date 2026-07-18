using System.Text.Json;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;

namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public sealed class DocumentIntegrationEventDispatcher(IEventBus eventBus)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public void Dispatch(BrokeredIntegrationEvent brokeredEvent)
    {
        switch (brokeredEvent.EventType)
        {
            case nameof(DocumentRegisteredIntegrationEvent):
                {
                    var integrationEvent = DeserializePayload<DocumentRegisteredIntegrationEvent>(brokeredEvent);
                    eventBus.Publish(new DocumentRegisteredEvent(
                        integrationEvent.Id,
                        integrationEvent.OccurredOn,
                        integrationEvent.ClaimNumber,
                        integrationEvent.DocumentType,
                        integrationEvent.FileName));
                    break;
                }

            case nameof(DocumentVerifiedIntegrationEvent):
                {
                    var integrationEvent = DeserializePayload<DocumentVerifiedIntegrationEvent>(brokeredEvent);
                    eventBus.Publish(new DocumentVerifiedEvent(
                        integrationEvent.Id,
                        integrationEvent.OccurredOn,
                        integrationEvent.ClaimNumber,
                        integrationEvent.DocumentType,
                        integrationEvent.FileName));
                    break;
                }

            case nameof(DocumentRejectedIntegrationEvent):
                {
                    var integrationEvent = DeserializePayload<DocumentRejectedIntegrationEvent>(brokeredEvent);
                    eventBus.Publish(new DocumentRejectedEvent(
                        integrationEvent.Id,
                        integrationEvent.OccurredOn,
                        integrationEvent.ClaimNumber,
                        integrationEvent.DocumentType,
                        integrationEvent.FileName,
                        integrationEvent.Reason));
                    break;
                }

            default:
                throw new InvalidOperationException($"Unsupported brokered document event type '{brokeredEvent.EventType}'.");
        }
    }

    private static TEvent DeserializePayload<TEvent>(BrokeredIntegrationEvent brokeredEvent)
    {
        var integrationEvent = JsonSerializer.Deserialize<TEvent>(brokeredEvent.Payload, JsonOptions);

        return integrationEvent
            ?? throw new InvalidOperationException($"Could not deserialize payload for '{brokeredEvent.EventType}'.");
    }
}
