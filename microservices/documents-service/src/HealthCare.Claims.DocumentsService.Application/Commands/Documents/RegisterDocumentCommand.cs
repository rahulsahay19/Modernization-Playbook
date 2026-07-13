using HealthCare.Claims.DocumentsService.Domain.Enums;

namespace HealthCare.Claims.DocumentsService.Application.Commands.Documents;

public sealed record RegisterDocumentCommand(
    string ClaimNumber,
    ClaimDocumentType DocumentType,
    string FileName,
    string StorageReference,
    string? Notes);
