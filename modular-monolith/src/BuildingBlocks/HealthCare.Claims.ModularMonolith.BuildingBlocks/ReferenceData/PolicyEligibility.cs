namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData
{
    public sealed record PolicyEligibility
    (
        string PolicyNumber,
        bool IsActive,
        decimal AvailableBalance,
        DateOnly EffectiveFrom,
        DateOnly EffectiveTo
    );
}
