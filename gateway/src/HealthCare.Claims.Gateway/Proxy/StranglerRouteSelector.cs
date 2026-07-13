using HealthCare.Claims.Gateway.Configuration;

namespace HealthCare.Claims.Gateway.Proxy
{
    public sealed class StranglerRouteSelector
    {
        public StranglerRoute? Select(PathString path, IReadOnlyCollection<StranglerRoute> routes)
        {
            var requestPath = path.Value ?? string.Empty;

            return routes
                   .Where(route => requestPath.StartsWith(route.PathPrefix, StringComparison.OrdinalIgnoreCase))
                   .OrderByDescending(route => route.PathPrefix.Length)
                   .FirstOrDefault();
        }
    }
}
