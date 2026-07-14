namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents
{
    public sealed record DocumentVerifiedIntegrationEvent(
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string DocumentType,
        string FileName);
}
