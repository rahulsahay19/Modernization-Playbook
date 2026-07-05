using HealthCare.Claims.Modules.Membership.Domain;

namespace HealthCare.Claims.Modules.Membership.Application
{
    public sealed class MembershipApplicationService(IMemberRepository members)
    {
        public IReadOnlyCollection<MemberSummary> List(MemberStatus? status = null, string? policyNumber = null)
        {
            var query = members.List().AsEnumerable();
            if (status is not null)
            { 
                query = query.Where(member => member.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(policyNumber))
            {
                query = query.Where(member =>
                string.Equals(member.PolicyNumber, policyNumber, StringComparison.OrdinalIgnoreCase));
            }
            return query
                .OrderBy(member => member.FullName)
                .Select(MemberSummary.FromMember)
                .ToArray();
        }

        public MemberSummary? GetById(Guid id)
        {
            var member = members.GetById(id);
            return member is null ? null : MemberSummary.FromMember(member); 
        }
    }
}
