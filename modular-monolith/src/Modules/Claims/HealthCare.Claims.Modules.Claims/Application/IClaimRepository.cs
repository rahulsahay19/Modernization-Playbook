using HealthCare.Claims.Modules.Claims.Domain;

namespace HealthCare.Claims.Modules.Claims.Application
{
    public interface IClaimRepository
    {
        IReadOnlyCollection<Claim> List();
        Claim? GetById(Guid id);
        void Add(Claim claim);
    }
}
