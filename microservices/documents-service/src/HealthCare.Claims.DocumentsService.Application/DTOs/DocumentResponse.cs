using HealthCare.Claims.DocumentsService.Domain.Entities;

namespace HealthCare.Claims.DocumentsService.Application.DTOs;

public sealed record DocumentResponse(
    Guid Id,
    string ClaimNumber,
    string DocumentType,
    string FileName,
    string StorageReference,
    string Status,
    string? Notes,
    DateTimeOffset ReceivedOn)
{
    public static DocumentResponse FromDocument(ClaimDocument document) =>
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
