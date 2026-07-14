namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record DocumentRejectedEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string DocumentType,
        string FileName,
        string Reason
    ) : IIntegrationEvent;
}
