using HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData;
using HealthCare.Claims.Modules.ProviderNetwork.Domain;

namespace HealthCare.Claims.Modules.ProviderNetwork.Application
{
    public sealed class ProviderNetworkReader(IProviderRepository providers) : IProviderNetworkReader
    {
        public ProviderNetworkEligibility? GetByProviderCode(string providerCode)
        {
            var provider = providers.GetByCode(providerCode);

            return provider is null
                ? null
                : new ProviderNetworkEligibility(
                    provider.ProviderCode,
                    provider.Name,
                    provider.NetworkTier.ToString(),
                    provider.Status == ProviderStatus.Active,
                    provider.CashlessEnabled);
        }
    }
}
