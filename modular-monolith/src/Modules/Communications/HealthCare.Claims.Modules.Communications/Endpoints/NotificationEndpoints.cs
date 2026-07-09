using HealthCare.Claims.Modules.Communications.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HealthCare.Claims.Modules.Communications.Endpoints
{
    public static class NotificationEndpoints
    {
        public static void Map(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/notifications").WithTags("Communications");
            group.MapGet("/", (NotificationApplicationService notifications, string? recipient, string? sourceEvent) =>
                Results.Ok(notifications.List(recipient, sourceEvent)));
        }
    }
}
