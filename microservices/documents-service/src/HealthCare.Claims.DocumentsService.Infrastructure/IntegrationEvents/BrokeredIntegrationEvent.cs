namespace HealthCare.Claims.DocumentsService.Infrastructure.IntegrationEvents;

public sealed record BrokeredIntegrationEvent(
    Guid MessageId,
    string EventType,
    DateTimeOffset OccurredOn,
    string Payload);
