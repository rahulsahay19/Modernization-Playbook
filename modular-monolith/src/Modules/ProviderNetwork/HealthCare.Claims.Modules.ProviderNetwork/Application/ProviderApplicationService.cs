using HealthCare.Claims.Modules.ProviderNetwork.Domain;

namespace HealthCare.Claims.Modules.ProviderNetwork.Application
{
    public sealed class ProviderApplicationService(IProviderRepository providers)
    {
        public IReadOnlyCollection<ProviderSummary> List(
            string? city = null,
            string? speciality = null,
            bool? cashlessEnabled = null,
            ProviderNetworkTier? networkTier = null)
        {
            var query = providers.List().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(city))
            { 
                query = query.Where(provider => string.Equals(provider.City, city, StringComparison.OrdinalIgnoreCase));
            }
            if(!string.IsNullOrWhiteSpace(speciality))
            {
                query = query.Where(provider => string.Equals(provider.Speaciality, speciality, StringComparison.OrdinalIgnoreCase));
            }
            if(cashlessEnabled is not null)
            {
                query = query.Where(provider => provider.CashlessEnabled == cashlessEnabled);
            }
            if (networkTier is not null)
            {
                query = query.Where(provider => provider.NetworkTier == networkTier);
            }
            return query
                .OrderBy(provider => provider.City)
                .ThenBy(provider => provider.Name)
                .Select(ProviderSummary.FromProvider)
                .ToArray();
        }

        public ProviderSummary? GetById(Guid id)
        {
            var provider = providers.GetById(id);
            return provider is null ? null : ProviderSummary.FromProvider(provider);
        }
    }
}
