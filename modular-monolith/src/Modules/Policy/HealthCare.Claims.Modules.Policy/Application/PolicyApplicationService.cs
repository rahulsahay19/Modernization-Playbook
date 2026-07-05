using HealthCare.Claims.Modules.Policy.Domain;

namespace HealthCare.Claims.Modules.Policy.Application
{
    public sealed class PolicyApplicationService(IPolicyRepository policies)
    {
        public IReadOnlyCollection<PolicySummary> List(PolicyStatus? status = null)
        {
            var query = policies.List().AsEnumerable();
            if(status is not null)
            {
                query = query.Where(policy  => policy.Status == status);
            }
            return query
                    .OrderBy(policy => policy.ProviderName)
                    .ThenBy(policy => policy.PolicyNumber)
                    .Select(PolicySummary.FromPolicy)
                    .ToArray();
        }

        public PolicySummary? GetById(Guid id)
        {
            var policy = policies.GetById(id);
            return policy is null ? null : PolicySummary.FromPolicy(policy);
        }
    }
}
