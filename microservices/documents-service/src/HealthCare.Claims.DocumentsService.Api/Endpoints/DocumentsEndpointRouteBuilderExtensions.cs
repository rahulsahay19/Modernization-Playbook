namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class DocumentsEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/documents").WithTags("Documents");

        group.MapListDocumentsEndpoint();
        group.MapGetDocumentEndpoint();
        group.MapRegisterDocumentEndpoint();
        group.MapVerifyDocumentEndpoint();
        group.MapRejectDocumentEndpoint();

        return endpoints;
    }
}
