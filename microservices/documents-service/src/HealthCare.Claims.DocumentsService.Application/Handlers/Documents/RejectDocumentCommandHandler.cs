using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts;

namespace HealthCare.Claims.DocumentsService.Application.Handlers.Documents;

public sealed class RejectDocumentCommandHandler(
    IClaimDocumentRepository repository,
    IIntegrationEventPublisher eventPublisher)
    : ICommandHandler<RejectDocumentCommand, DocumentCommandResult>
{
    public async Task<DocumentCommandResult> Handle(RejectDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = repository.GetById(command.Id);

        if (document is null)
        {
            return DocumentCommandResult.Missing();
        }

        if (string.IsNullOrWhiteSpace(command.Notes))
        {
            return DocumentCommandResult.BadRequest("Rejection notes are required.");
        }

        document.Reject(command.Notes);
        await eventPublisher.PublishDocumentRejectedAsync(
            new DocumentRejectedIntegrationEvent(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                document.ClaimNumber,
                document.DocumentType.ToString(),
                document.FileName,
                command.Notes), cancellationToken);

        return DocumentCommandResult.Success(DocumentResponse.FromDocument(document));
    }
}
