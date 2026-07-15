namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents;

public sealed record BrokeredIntegrationEvent(
    Guid MessageId,
    string EventType,
    DateTimeOffset OccurredOn,
    string Payload);
