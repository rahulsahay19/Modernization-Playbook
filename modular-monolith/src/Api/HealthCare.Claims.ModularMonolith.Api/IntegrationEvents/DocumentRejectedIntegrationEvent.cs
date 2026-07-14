namespace HealthCare.Claims.ModularMonolith.Api.IntegrationEvents
{
    public sealed record DocumentRejectedIntegrationEvent(
    Guid Id,
    DateTimeOffset OccurredOn,
    string ClaimNumber,
    string DocumentType,
    string FileName,
    string Reason);
}
