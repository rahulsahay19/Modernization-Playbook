using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.Queries.Documents;

namespace HealthCare.Claims.DocumentsService.Application.Handlers.Documents;

public sealed class ListDocumentsQueryHandler(IClaimDocumentRepository repository)
    : IQueryHandler<ListDocumentsQuery, IReadOnlyCollection<DocumentResponse>>
{
    public IReadOnlyCollection<DocumentResponse> Handle(ListDocumentsQuery query)
    {
        return repository.List(query.ClaimNumber)
            .OrderByDescending(document => document.ReceivedOn)
            .Select(DocumentResponse.FromDocument)
            .ToArray();
    }
}
