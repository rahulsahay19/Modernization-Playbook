namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents
{
    public sealed record PaymentSettledEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string PaymentNumber,
        string ClaimNumber,
        string PayeeName,
        decimal Amount
    ) : IIntegrationEvent;
}
