using HealthCare.Claims.Modules.Policy.Domain;

namespace HealthCare.Claims.Modules.Policy.Application
{
    public sealed record PolicySummary
    (
        Guid Id,
        string PolicyNumber,
        string ProviderName,
        string ProductName,
        string PlanType,
        string Status,
        decimal SumInsured,
        decimal AvailableBalance,
        DateOnly EffectiveFrom,
        DateOnly EffectiveTo
    )
    {
        public static PolicySummary FromPolicy(InsurancePolicy policy) =>
            new(
                policy.Id,
                policy.PolicyNumber,
                policy.ProviderName,
                policy.ProductName,
                policy.PlanType.ToString(),
                policy.Status.ToString(),
                policy.SumInsured,
                policy.AvailableBalance,
                policy.EffectiveFrom,
                policy.EffectiveTo
                );
    }
}
