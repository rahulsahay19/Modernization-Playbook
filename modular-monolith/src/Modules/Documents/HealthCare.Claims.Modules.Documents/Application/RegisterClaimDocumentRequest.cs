using HealthCare.Claims.Modules.Documents.Domain;

namespace HealthCare.Claims.Modules.Documents.Application
{
    public sealed record RegisterClaimDocumentRequest
    (
        string ClaimNumber,
        ClaimDocumentType DocumentType,
        string FileName,
        string StorageReference,
        string? Notes
    );
}
