using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;

namespace HealthCare.Claims.DocumentsService.Application.Handlers.Documents;

public sealed class VerifyDocumentCommandHandler(IClaimDocumentRepository repository)
    : ICommandHandler<VerifyDocumentCommand, DocumentCommandResult>
{
    public DocumentCommandResult Handle(VerifyDocumentCommand command)
    {
        var document = repository.GetById(command.Id);

        if (document is null)
        {
            return DocumentCommandResult.Missing();
        }

        document.Verify(command.Notes);

        return DocumentCommandResult.Success(DocumentResponse.FromDocument(document));
    }
}
