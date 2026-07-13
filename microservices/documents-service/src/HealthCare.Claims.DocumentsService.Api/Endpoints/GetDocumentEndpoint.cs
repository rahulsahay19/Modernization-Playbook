using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.Queries.Documents;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class GetDocumentEndpoint
{
    public static RouteGroupBuilder MapGetDocumentEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", (
            Guid id,
            IQueryHandler<GetDocumentQuery, DocumentResponse?> handler) =>
        {
            var document = handler.Handle(new GetDocumentQuery(id));

            return document is null
                ? Results.NotFound()
                : Results.Ok(document);
        });

        return group;
    }
}
