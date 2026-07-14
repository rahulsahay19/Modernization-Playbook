namespace HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts
{
    public sealed record DocumentVerifiedIntegrationEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string DocumentType,
        string FileName
    );
}
