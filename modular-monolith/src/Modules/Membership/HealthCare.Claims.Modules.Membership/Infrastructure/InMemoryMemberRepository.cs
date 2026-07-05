using HealthCare.Claims.Modules.Membership.Application;
using HealthCare.Claims.Modules.Membership.Domain;

namespace HealthCare.Claims.Modules.Membership.Infrastructure
{
    public sealed class InMemoryMemberRepository : IMemberRepository
    {
        private static readonly Member[] SeedMembers =
            [
                new(
            Guid.Parse("20000000-0000-0000-0000-000000000001"),
            "MEM-10001",
            "Aarav Sharma",
            new DateOnly(1988, 5, 18),
            "Male",
            "+91-9876543210",
            "Gurugram",
            "Haryana",
            "HDFC-ERGO-FAM-1001",
            MemberStatus.Active),
        new(
            Guid.Parse("20000000-0000-0000-0000-000000000002"),
            "MEM-10002",
            "Meera Iyer",
            new DateOnly(1991, 11, 2),
            "Female",
            "+91-9845012345",
            "Bengaluru",
            "Karnataka",
            "HDFC-ERGO-FAM-1001",
            MemberStatus.Active),
        new(
            Guid.Parse("20000000-0000-0000-0000-000000000003"),
            "MEM-20450",
            "Suresh Menon",
            new DateOnly(1959, 3, 27),
            "Male",
            "+91-9820011122",
            "Mumbai",
            "Maharashtra",
            "STAR-SENIOR-2045",
            MemberStatus.PendingKyc),
        new(
            Guid.Parse("20000000-0000-0000-0000-000000000004"),
            "MEM-77881",
            "Priya Nair",
            new DateOnly(1984, 8, 9),
            "Female",
            "+91-9740012345",
            "Kochi",
            "Kerala",
            "NI-CORP-7788",
            MemberStatus.Active)
            ];
        public Member? GetById(Guid id) => SeedMembers.FirstOrDefault(member => member.Id == id);

        public IReadOnlyCollection<Member> List() => SeedMembers;

        public IReadOnlyCollection<Member> ListByPolicyNumber(string policyNumber) =>
            SeedMembers
                .Where(member=> string.Equals(member.PolicyNumber, policyNumber, StringComparison.OrdinalIgnoreCase))
                .ToArray();
    }
}
