namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record ClaimApprovedEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string MemberNumber,
        decimal ApprovedAmount,
        string Reason
    ) : IIntegrationEvent;
}
