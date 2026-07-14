namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record DocumentRegisteredEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string DocumentType,
        string FileName
    ) : IIntegrationEvent;
}
