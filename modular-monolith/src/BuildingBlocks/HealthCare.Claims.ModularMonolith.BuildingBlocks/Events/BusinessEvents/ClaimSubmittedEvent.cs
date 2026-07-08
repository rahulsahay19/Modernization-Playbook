namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record ClaimSubmittedEvent
   (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string MemberNumber,
        string PolicyNumber,
        decimal TotalAmount
    ) : IIntegrationEvent;
}
