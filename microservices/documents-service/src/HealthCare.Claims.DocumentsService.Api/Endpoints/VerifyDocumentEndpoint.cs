using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class VerifyDocumentEndpoint
{
    public static RouteGroupBuilder MapVerifyDocumentEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/verify", (
            Guid id,
            DocumentDecisionRequest request,
            ICommandHandler<VerifyDocumentCommand, DocumentCommandResult> handler) =>
        {
            var result = handler.Handle(new VerifyDocumentCommand(id, request.Notes));

            return result.NotFound
                ? Results.NotFound()
                : Results.Ok(result.Document);
        });

        return group;
    }
}
