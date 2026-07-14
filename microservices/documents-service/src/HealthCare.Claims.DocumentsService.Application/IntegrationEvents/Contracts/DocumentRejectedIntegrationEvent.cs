namespace HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts
{
    public sealed record DocumentRejectedIntegrationEvent
    (
        Guid Id,
        DateTimeOffset OccurredOn,
        string ClaimNumber,
        string DocumentType,
        string FileName,
        string Reason
    );
}
