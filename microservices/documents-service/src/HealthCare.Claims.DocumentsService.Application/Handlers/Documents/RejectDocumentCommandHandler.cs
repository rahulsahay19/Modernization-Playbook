using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;

namespace HealthCare.Claims.DocumentsService.Application.Handlers.Documents;

public sealed class RejectDocumentCommandHandler(IClaimDocumentRepository repository)
    : ICommandHandler<RejectDocumentCommand, DocumentCommandResult>
{
    public DocumentCommandResult Handle(RejectDocumentCommand command)
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

        return DocumentCommandResult.Success(DocumentResponse.FromDocument(document));
    }
}
