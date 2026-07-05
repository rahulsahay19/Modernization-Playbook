using HealthCare.Claims.Modules.Policy.Application;
using HealthCare.Claims.Modules.Policy.Domain;

namespace HealthCare.Claims.Modules.Policy.Infrastructure
{
    public sealed class InMemoryPolicyRepository : IPolicyRepository
    {
        private static readonly InsurancePolicy[] SeedPolicies =
            [
                new(
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            "HDFC-ERGO-FAM-1001",
            "HDFC ERGO General Insurance",
            "Optima Secure",
            PolicyPlanType.FamilyFloater,
            PolicyStatus.Active,
            1500000m,
            1265000m,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31)),
        new(
            Guid.Parse("10000000-0000-0000-0000-000000000002"),
            "STAR-SENIOR-2045",
            "Star Health and Allied Insurance",
            "Senior Citizens Red Carpet",
            PolicyPlanType.SeniorCitizen,
            PolicyStatus.GracePeriod,
            700000m,
            682000m,
            new DateOnly(2025, 7, 1),
            new DateOnly(2026, 6, 30)),
        new(
            Guid.Parse("10000000-0000-0000-0000-000000000003"),
            "NI-CORP-7788",
            "National Insurance Company",
            "Corporate Mediclaim",
            PolicyPlanType.CorporateGroup,
            PolicyStatus.Active,
            2500000m,
            2310000m,
            new DateOnly(2026, 4, 1),
            new DateOnly(2027, 3, 31))
            ];
        public InsurancePolicy? GetById(Guid id) => SeedPolicies.FirstOrDefault(policy => policy.Id == id);

        public InsurancePolicy? GetByPolicyNumber(string policyNumber) =>
            SeedPolicies.FirstOrDefault(policy=> string.Equals(policy.PolicyNumber, policyNumber, StringComparison.OrdinalIgnoreCase));


        public IReadOnlyCollection<InsurancePolicy> List() => SeedPolicies;
    }
}
