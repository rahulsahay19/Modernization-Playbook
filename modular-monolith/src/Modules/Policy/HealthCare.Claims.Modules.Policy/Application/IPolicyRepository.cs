using HealthCare.Claims.Modules.Policy.Domain;

namespace HealthCare.Claims.Modules.Policy.Application
{
    public interface IPolicyRepository
    {
        IReadOnlyCollection<InsurancePolicy> List();
        InsurancePolicy? GetById(Guid id);
        InsurancePolicy? GetByPolicyNumber(string policyNumber);
    }
}
