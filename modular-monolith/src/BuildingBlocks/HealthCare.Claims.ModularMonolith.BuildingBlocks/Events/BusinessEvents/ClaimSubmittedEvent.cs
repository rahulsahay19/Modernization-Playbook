namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record ClaimSubmittedEvent
   (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string PolicyNumber,
        string MemberNumber,        
        decimal TotalAmount
    ) : IIntegrationEvent;
}
