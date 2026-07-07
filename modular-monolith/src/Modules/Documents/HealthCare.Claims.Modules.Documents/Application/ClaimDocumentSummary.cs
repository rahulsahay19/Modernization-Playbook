using HealthCare.Claims.Modules.Documents.Domain;

namespace HealthCare.Claims.Modules.Documents.Application
{
    public sealed record ClaimDocumentSummary(
        Guid Id,
        string ClaimNumber,
        string DocumentType,
        string FileName,
        string StorageReference,
        string Status,
        string? Notes,
        DateTimeOffset ReceivedOn)
    {
        public static ClaimDocumentSummary FromDocument(ClaimDocument document) =>
            new(
                document.Id,
                document.ClaimNumber,
                document.DocumentType.ToString(),
                document.FileName,
                document.StorageReference,
                document.Status.ToString(),
                document.Notes,
                document.ReceivedOn);
    }
}
