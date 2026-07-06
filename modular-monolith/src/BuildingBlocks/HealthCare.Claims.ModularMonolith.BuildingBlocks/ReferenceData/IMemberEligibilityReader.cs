namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData
{
    public interface IMemberEligibilityReader
    {
        MemberEligibility? GetByMemberNumber(string memberNumber);
    }
}
