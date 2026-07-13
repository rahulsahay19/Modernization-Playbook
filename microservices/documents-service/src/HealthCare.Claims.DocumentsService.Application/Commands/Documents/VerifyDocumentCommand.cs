namespace HealthCare.Claims.DocumentsService.Application.Commands.Documents;

public sealed record VerifyDocumentCommand(Guid Id, string? Notes);
