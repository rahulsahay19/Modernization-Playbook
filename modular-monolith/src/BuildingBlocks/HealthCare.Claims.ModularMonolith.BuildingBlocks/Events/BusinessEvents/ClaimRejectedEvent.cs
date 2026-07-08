namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record ClaimRejectedEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string MemberNumber,
        string Reason
    ) : IIntegrationEvent;
}
