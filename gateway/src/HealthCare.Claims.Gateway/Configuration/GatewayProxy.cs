using HealthCare.Claims.Gateway.Proxy;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace HealthCare.Claims.Gateway.Configuration
{
    public sealed class GatewayProxy(
        IOptions<GatewayOptions> options,
        IHttpClientFactory httpClientFactory,
        StranglerRouteSelector routeSelector,
        ILogger<GatewayProxy> logger)
    {
        public async Task Forward(HttpContext context, CancellationToken cancellationToken)
        {
            var route = routeSelector.Select(context.Request.Path, options.Value.Routes);
            if(route is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "No strangler route is configured for this API path.",
                    path = context.Request.Path.Value
                }, cancellationToken);
                return;
            }

            var destinationUri = BuildDestinationUri(context.Request, route);
            using var proxyRequest = CreateProxyRequest(context, destinationUri);
            var client = httpClientFactory.CreateClient("strangler-gateway");
            logger.LogInformation(
                "Gateway forwarding {Method} {Path} to {Upstream} at {DestinationUri}",
                context.Request.Method,
                context.Request.Path,
                route.UpstreamName,
                destinationUri);

            using var response = await client.SendAsync(
                proxyRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            context.Response.StatusCode = (int)response.StatusCode;
            CopyResponseHeader(response, context.Response);
            context.Response.Headers["X-Strangler-Route"] = route.RouteId;
            context.Response.Headers["X-Upstream-Service"] = route.UpstreamName;
        }

        private object BuildDestinationUri(HttpRequest request, StranglerRoute route)
        {
            var destinationBase = new Uri(route.DestinationBaseUrl.TrimEnd('/') + "/");
            var pathAndQuery = request.Path + request.QueryString;
            return new Uri(destinationBase, pathAndQuery.TrimStart('/'));
        }

        private object CreateProxyRequest(HttpContext context, Uri destinationUri)
        {
            var request = context.Request;
            var proxyRequest = new HttpRequestMessage(new HttpMethod(request.Method), destinationUri);
            if(HttpMethods.IsPost(request.Method) ||
               HttpMethods.IsPut(request.Method)||
               HttpMethods.IsPatch(request.Method))
            {
                proxyRequest.Content = new StreamContent(request.Body);
                if(!string.IsNullOrWhiteSpace(request.ContentType))
                {
                    proxyRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);
                }
            }
            foreach (var header in request.Headers)
            {
                if(header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if(!proxyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    proxyRequest.Content.Headers?.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }
            proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Host", request.Host.Value);
            proxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Proto", request.Scheme);
            return proxyRequest;
        }

       

        private void CopyResponseHeader(object response1, HttpResponse response2)
        {
            throw new NotImplementedException();
        }
    }
}
