using HealthCare.Claims.Modules.ProviderNetwork.Application;
using HealthCare.Claims.Modules.ProviderNetwork.Domain;

namespace HealthCare.Claims.Modules.ProviderNetwork.Infrastructure
{
    public sealed class InMemoryProviderRepository : IProviderRepository
    {
        private static readonly HealthCareProvider[] SeedProviders =
            [
                new(
            Guid.Parse("30000000-0000-0000-0000-000000000001"),
            "BLR-NH-001",
            "Narayana Health City",
            "Cardiology",
            "Bengaluru",
            "Karnataka",
            ProviderNetworkTier.Preferred,
            ProviderStatus.Active,
            true),
        new(
            Guid.Parse("30000000-0000-0000-0000-000000000002"),
            "GGM-FM-014",
            "Fortis Memorial Research Institute",
            "Orthopedics",
            "Gurugram",
            "Haryana",
            ProviderNetworkTier.Preferred,
            ProviderStatus.Active,
            true),
        new(
            Guid.Parse("30000000-0000-0000-0000-000000000003"),
            "MUM-LH-022",
            "Lilavati Hospital and Research Centre",
            "Oncology",
            "Mumbai",
            "Maharashtra",
            ProviderNetworkTier.Standard,
            ProviderStatus.Active,
            true),
        new(
            Guid.Parse("30000000-0000-0000-0000-000000000004"),
            "KOC-AM-008",
            "Aster Medcity",
            "Neurology",
            "Kochi",
            "Kerala",
            ProviderNetworkTier.Standard,
            ProviderStatus.Onboarding,
            false)
            ];
        public HealthCareProvider? GetByCode(string providerCode) =>
            SeedProviders.FirstOrDefault(provider => string.Equals(provider.ProviderCode, providerCode, StringComparison.OrdinalIgnoreCase));

        public HealthCareProvider? GetById(Guid id) => 
            SeedProviders.FirstOrDefault(provider => provider.Id == id);

        public IReadOnlyCollection<HealthCareProvider> List() => SeedProviders;
    }
}
