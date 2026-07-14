using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class RejectDocumentEndpoint
{
    public static RouteGroupBuilder MapRejectDocumentEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/reject", async (
            Guid id,
            DocumentDecisionRequest request,
            ICommandHandler<RejectDocumentCommand, DocumentCommandResult> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new RejectDocumentCommand(id, request.Notes), cancellationToken);

            if (result.NotFound)
            {
                return Results.NotFound();
            }

            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Ok(result.Document);
        });

        return group;
    }
}
