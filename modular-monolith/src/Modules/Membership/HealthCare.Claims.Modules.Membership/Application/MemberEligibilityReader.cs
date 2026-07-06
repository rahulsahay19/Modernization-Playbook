using HealthCare.Claims.ModularMonolith.BuildingBlocks.ReferenceData;
using HealthCare.Claims.Modules.Membership.Domain;

namespace HealthCare.Claims.Modules.Membership.Application
{
    public sealed class MemberEligibilityReader(IMemberRepository members) : IMemberEligibilityReader
    {
        public MemberEligibility? GetByMemberNumber(string memberNumber)
        {
            var member = members.List()
                            .FirstOrDefault(candidate => string.Equals(candidate.MemberNumber, memberNumber, StringComparison.OrdinalIgnoreCase));
            return member is null
                ? null
                : new MemberEligibility(
                        member.MemberNumber,
                        member.FullName,
                        member.PolicyNumber,
                        member.Status == MemberStatus.Active
                    );
        }
    }
}
