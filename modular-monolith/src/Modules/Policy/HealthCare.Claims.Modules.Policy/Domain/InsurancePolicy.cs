namespace HealthCare.Claims.Modules.Policy.Domain
{
    public sealed record InsurancePolicy
    (
        Guid Id,
        string PolicyNumber,
        string ProviderName,
        string ProductName,
        PolicyPlanType PlanType,
        PolicyStatus Status,
        decimal SumInsured,
        decimal AvailableBalance,
        DateOnly EffectiveFrom,
        DateOnly EffectiveTo
    );
}
