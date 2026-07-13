using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.DTOs;
using HealthCare.Claims.DocumentsService.Application.Queries.Documents;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class ListDocumentsEndpoint
{
    public static RouteGroupBuilder MapListDocumentsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", (
            IQueryHandler<ListDocumentsQuery, IReadOnlyCollection<DocumentResponse>> handler,
            string? claimNumber) =>
            Results.Ok(handler.Handle(new ListDocumentsQuery(claimNumber))));

        return group;
    }
}
