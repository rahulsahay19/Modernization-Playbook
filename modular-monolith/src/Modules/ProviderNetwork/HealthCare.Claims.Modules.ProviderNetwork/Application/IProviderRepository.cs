using HealthCare.Claims.Modules.ProviderNetwork.Domain;

namespace HealthCare.Claims.Modules.ProviderNetwork.Application
{
    public interface IProviderRepository
    {
        IReadOnlyCollection<HealthCareProvider> List();
        HealthCareProvider? GetById(Guid id);
        HealthCareProvider? GetByCode(string providerCode);
    }
}
