namespace HealthCare.Claims.DocumentsService.Application.Commands.Documents;

public sealed record RejectDocumentCommand(Guid Id, string? Notes);
