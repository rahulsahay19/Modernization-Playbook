using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents;
using HealthCare.Claims.DocumentsService.Application.IntegrationEvents.Contracts;
using HealthCare.Claims.DocumentsService.Domain.Entities;
using HealthCare.Claims.DocumentsService.Domain.Enums;

namespace HealthCare.Claims.DocumentsService.Application.Handlers.Documents;

public sealed class RegisterDocumentCommandHandler(
    IClaimDocumentRepository repository,
    IIntegrationEventPublisher eventPublisher)
    : ICommandHandler<RegisterDocumentCommand, DocumentCommandResult>
{
    public async Task<DocumentCommandResult> Handle(RegisterDocumentCommand command, CancellationToken cancellationToken)
    {
        var validationError = Validate(command);
        if (validationError is not null)
        {
            return DocumentCommandResult.BadRequest(validationError);
        }

        var document = new ClaimDocument(
            Guid.NewGuid(),
            command.ClaimNumber,
            command.DocumentType,
            command.FileName,
            command.StorageReference,
            ClaimDocumentStatus.Received,
            command.Notes);

        repository.Add(document);
        await eventPublisher.PublishDocumentRegisteredAsync(
            new DocumentRegisteredIntegrationEvent(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                document.ClaimNumber,
                document.DocumentType.ToString(),
                document.FileName), cancellationToken);

        return DocumentCommandResult.Success(DocumentResponse.FromDocument(document));
    }

    private static string? Validate(RegisterDocumentCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.ClaimNumber))
        {
            return "Claim number is required.";
        }

        if (string.IsNullOrWhiteSpace(command.FileName))
        {
            return "File name is required.";
        }

        return string.IsNullOrWhiteSpace(command.StorageReference)
            ? "Storage reference is required."
            : null;
    }
}
