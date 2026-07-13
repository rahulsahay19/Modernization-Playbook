namespace HealthCare.Claims.Gateway.Configuration
{
    public sealed record StranglerRoute
    (
        string RouteId,
        string Capability,
        string PathPrefix,
        string UpstreamName,
        string DestinationBaseUrl,
        bool Extracted,
        string RoutePurpose
    );
}
