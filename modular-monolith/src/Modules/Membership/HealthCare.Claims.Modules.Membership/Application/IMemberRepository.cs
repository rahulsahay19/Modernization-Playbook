using HealthCare.Claims.Modules.Membership.Domain;

namespace HealthCare.Claims.Modules.Membership.Application
{
    public interface IMemberRepository
    {
        IReadOnlyCollection<Member> List();
        Member? GetById(Guid id);
        IReadOnlyCollection<Member> ListByPolicyNumber(string policyNumber);
    }
}
