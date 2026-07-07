using HealthCare.Claims.Modules.Documents.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Documents.Endpoints
{
    public static class DocumentEndpoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/documents").WithTags("Documents");

            group.MapGet("/", (DocumentApplicationService documents, string? claimNumber) =>
                Results.Ok(documents.List(claimNumber)));

            group.MapGet("/{id:guid}", (Guid id, DocumentApplicationService documents) =>
            {
                var document = documents.GetById(id);

                return document is null ? Results.NotFound() : Results.Ok(document);
            });

            group.MapPost("/", (RegisterClaimDocumentRequest request, DocumentApplicationService documents) =>
            {
                try
                {
                    var document = documents.Register(request);

                    return Results.Created($"/api/documents/{document.Id}", document);
                }
                catch (InvalidOperationException exception)
                {
                    return Results.BadRequest(new { error = exception.Message });
                }
            });

            group.MapPost("/{id:guid}/verify", (Guid id, DocumentDecisionRequest request, DocumentApplicationService documents) =>
            {
                var document = documents.Verify(id, request);

                return document is null ? Results.NotFound() : Results.Ok(document);
            });

            group.MapPost("/{id:guid}/reject", (Guid id, DocumentDecisionRequest request, DocumentApplicationService documents) =>
            {
                try
                {
                    var document = documents.Reject(id, request);

                    return document is null ? Results.NotFound() : Results.Ok(document);
                }
                catch (InvalidOperationException exception)
                {
                    return Results.BadRequest(new { error = exception.Message });
                }
            });
        }
    }
}

