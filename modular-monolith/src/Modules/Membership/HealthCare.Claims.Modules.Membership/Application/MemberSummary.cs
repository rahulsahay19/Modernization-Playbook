using HealthCare.Claims.Modules.Membership.Domain;

namespace HealthCare.Claims.Modules.Membership.Application
{
    public sealed record MemberSummary(
        Guid Id,
        string MemberNumber,
        string FullName,
        DateOnly DateOfBirth,
        string Gender,
        string MobileNumber,
        string City,
        string State,
        string PolicyNumber,
        string Status
        )
    {
        public static MemberSummary FromMember(Member member) =>
            new(
                member.Id,
                member.MemberNumber,
                member.FullName,
                member.DateOfBirth,
                member.Gender,
                member.MobileNumber,
                member.City,
                member.State,
                member.PolicyNumber,
                member.Status.ToString()
                );
    }
    
}
