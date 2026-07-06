using HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData;
using HealthCare.Claims.Modules.Policy.Domain;

namespace HealthCare.Claims.Modules.Policy.Application
{
    public class PolicyEligibilityReader(IPolicyRepository policies) : IPolicyEligibilityReader
    {
        public PolicyEligibility? GetByPolicyNumber(string policyNumber)
        {
            var policy = policies.GetByPolicyNumber(policyNumber);

            return policy is null
                ? null
                : new PolicyEligibility(
                        policy.PolicyNumber,
                        policy.Status is PolicyStatus.Active or PolicyStatus.GracePeriod,
                        policy.AvailableBalance,
                        policy.EffectiveFrom,
                        policy.EffectiveTo
                    );
        }
    }
}
