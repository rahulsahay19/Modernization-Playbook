using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts;

namespace HealthCare.Claims.DocumentsService.Application.Handlers.Documents;

public sealed class VerifyDocumentCommandHandler(
    IClaimDocumentRepository repository,
    IIntegrationEventPublisher eventPublisher)
    : ICommandHandler<VerifyDocumentCommand, DocumentCommandResult>
{
    public async Task<DocumentCommandResult> Handle(VerifyDocumentCommand command, CancellationToken cancellationToken)
    {
        var document = repository.GetById(command.Id);

        if (document is null)
        {
            return DocumentCommandResult.Missing();
        }

        document.Verify(command.Notes);
        await eventPublisher.PublishDocumentVerifiedAsync(
            new DocumentVerifiedIntegrationEvent(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                document.ClaimNumber,
                document.DocumentType.ToString(),
                document.FileName
                ), cancellationToken);

        return DocumentCommandResult.Success(DocumentResponse.FromDocument(document));
    }
}
