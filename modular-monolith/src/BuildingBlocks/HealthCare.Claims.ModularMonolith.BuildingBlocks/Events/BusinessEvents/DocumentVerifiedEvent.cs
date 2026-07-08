namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record DocumentVerifiedEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string DocumentType,
        string FileName
    ): IIntegrationEvent;
}
