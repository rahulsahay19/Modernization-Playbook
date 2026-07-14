namespace HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts
{
    public sealed record DocumentRegisteredIntegrationEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string DocumentType,
        string FileName
    );
}
