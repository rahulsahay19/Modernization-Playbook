using HealthCare.Claims.DocumentsService.Application.Abstractions;
using HealthCare.Claims.DocumentsService.Application.Commands.Documents;
using HealthCare.Claims.DocumentsService.Application.DTOs;

namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class RegisterDocumentEndpoint
{
    public static RouteGroupBuilder MapRegisterDocumentEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", (
            RegisterDocumentCommand command,
            ICommandHandler<RegisterDocumentCommand, DocumentCommandResult> handler) =>
        {
            var result = handler.Handle(command);

            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Created($"/api/documents/{result.Document!.Id}", result.Document);
        });

        return group;
    }
}
