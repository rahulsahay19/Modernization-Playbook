using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.Queries.Documents;

namespace HealthCare.Claims.DocumentsService.Application.Handlers.Documents;

public sealed class GetDocumentQueryHandler(IClaimDocumentRepository repository)
    : IQueryHandler<GetDocumentQuery, DocumentResponse?>
{
    public DocumentResponse? Handle(GetDocumentQuery query)
    {
        var document = repository.GetById(query.Id);

        return document is null
            ? null
            : DocumentResponse.FromDocument(document);
    }
}
