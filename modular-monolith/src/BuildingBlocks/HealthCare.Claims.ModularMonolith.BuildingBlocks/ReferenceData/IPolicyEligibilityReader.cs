namespace HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData
{
    public interface IPolicyEligibilityReader
    {
        PolicyEligibility? GetByPolicyNumber(string policyNumber);
    }
}
