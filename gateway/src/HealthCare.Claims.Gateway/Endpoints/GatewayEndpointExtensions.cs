using HealthCare.Claims.Gateway.Configuration;
using HealthCare.Claims.Gateway.Proxy;
using Microsoft.Extensions.Options;
using GatewayProxy = HealthCare.Claims.Gateway.Proxy.GatewayProxy;

namespace HealthCare.Claims.Gateway.Endpoints;

public static class GatewayEndpointExtensions
{
    public static void MapGatewayEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("/api/gateway/routes"));

        app.MapGet("/api/gateway/routes", (IOptions<GatewayOptions> options) =>
            Results.Ok(new
            {
                gateway = "HealthCare Strangler Gateway",
                strategy = "The frontend calls the gateway. The gateway decides which backend owns each capability.",
                routes = options.Value.Routes
            }));

        app.MapGet("/api/gateway/health", CheckGatewayHealth);

        app.MapMethods("/api/{**path}", GatewayHttpMethods.All, (
            HttpContext context,
            GatewayProxy proxy,
            CancellationToken cancellationToken) =>
            proxy.Forward(context, cancellationToken));
    }

    private static async Task<IResult> CheckGatewayHealth(
        IOptions<GatewayOptions> options,
        IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient("strangler-gateway");
        var destinations = options.Value.Routes
            .Select(route => new { route.UpstreamName, route.DestinationBaseUrl })
            .Distinct()
            .ToArray();

        var checks = new List<object>();
        foreach (var destination in destinations)
        {
            try
            {
                using var response = await client.GetAsync(
                    new Uri(new Uri(destination.DestinationBaseUrl), "/api/platform/modules"),
                    cancellationToken);

                checks.Add(new
                {
                    destination.UpstreamName,
                    destination.DestinationBaseUrl,
                    status = response.IsSuccessStatusCode ? "Reachable" : "Unhealthy",
                    statusCode = (int)response.StatusCode
                });
            }
            catch (HttpRequestException exception)
            {
                checks.Add(new
                {
                    destination.UpstreamName,
                    destination.DestinationBaseUrl,
                    status = "Unavailable",
                    error = exception.Message
                });
            }
        }

        return Results.Ok(new { checks });
    }
}
