namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData
{
    public sealed record MemberEligibility
    (
        string MemberName,
        string FullName,
        string PolicyNumber,
        bool isActive
    );
}
