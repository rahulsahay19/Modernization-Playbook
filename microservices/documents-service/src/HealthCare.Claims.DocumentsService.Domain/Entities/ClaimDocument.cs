using HealthCare.Claims.DocumentsService.Domain.Enums;

namespace HealthCare.Claims.DocumentsService.Domain.Entities;

public sealed class ClaimDocument(
    Guid id,
    string claimNumber,
    ClaimDocumentType documentType,
    string fileName,
    string storageReference,
    ClaimDocumentStatus status,
    string? notes = null)
{
    public Guid Id { get; } = id;

    public string ClaimNumber { get; } = claimNumber;

    public ClaimDocumentType DocumentType { get; } = documentType;

    public string FileName { get; } = fileName;

    public string StorageReference { get; } = storageReference;

    public ClaimDocumentStatus Status { get; private set; } = status;

    public string? Notes { get; private set; } = notes;

    public DateTimeOffset ReceivedOn { get; } = DateTimeOffset.UtcNow;

    public void Verify(string? notes)
    {
        Status = ClaimDocumentStatus.Verified;
        Notes = notes;
    }

    public void Reject(string notes)
    {
        Status = ClaimDocumentStatus.Rejected;
        Notes = notes;
    }
}
