namespace HealthCare.Claims.DocumentsService.Api.Endpoints;

public static class ServiceInfoEndpoints
{
    public static IEndpointRouteBuilder MapServiceInfoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", () => Results.Ok(new
        {
            service = "DocumentsService",
            status = "Healthy",
            checkedAt = DateTimeOffset.UtcNow
        }));

        endpoints.MapGet("/api/platform/modules", () => Results.Ok(new[]
        {
            new
            {
                name = "DocumentsService",
                description = "Extracted claim document intake and verification service.",
                extracted = true
            }
        }));

        return endpoints;
    }
}
