using HealthCare.Claims.Modules.ProviderNetwork.Domain;

namespace HealthCare.Claims.Modules.ProviderNetwork.Application
{
    public sealed record ProviderSummary(
        Guid Id,
        string ProviderCode,
        string Name,
        string Speaciality,
        string City,
        string State,
        string NetworkTier,
        string Status,
        bool CashlessEnabled
        )
    {
        public static ProviderSummary FromProvider(HealthCareProvider provider) =>
            new(
                provider.Id,
                provider.ProviderCode,
                provider.Name,
                provider.Speaciality,
                provider.City,
                provider.State,
                provider.NetworkTier.ToString(),
                provider.Status.ToString(),
                provider.CashlessEnabled);
    }
    
}
