using HealthCare.Claims.Modules.Claims.Application;
using HealthCare.Claims.Modules.Claims.Domain;

namespace HealthCare.Claims.Modules.Claims.Infrastructure
{
    public sealed class InMemoryClaimRepository : IClaimRepository
    {
        private readonly List<Claim> claims =
            [
            new(
            Guid.Parse("40000000-0000-0000-0000-000000000001"),
            "CLM-202606270001",
            "HDFC-ERGO-FAM-1001",
            "MEM-10001",
            "GGM-FM-014",
            ClaimType.Cashless,
            new DateOnly(2026, 6, 12),
            "Ligament injury",
            "Arthroscopy",
            [
                new ClaimLine("Hospital package", 78000m),
                new ClaimLine("Diagnostics", 8500m)
            ],
            ClaimStatus.UnderReview),
        new(
            Guid.Parse("40000000-0000-0000-0000-000000000002"),
            "CLM-202606270002",
            "NI-CORP-7788",
            "MEM-77881",
            "KOC-AM-008",
            ClaimType.Reimbursement,
            new DateOnly(2026, 6, 8),
            "Migraine with aura",
            "Neurology consultation",
            [
                new ClaimLine("Consultation", 2500m),
                new ClaimLine("MRI scan", 14500m)
            ],
            ClaimStatus.Submitted)
            ];
        
        private readonly object syncRoot = new();
        public void Add(Claim claim)
        {
            lock(syncRoot)
            {
                claims.Add(claim);
            }
        }

        public Claim? GetById(Guid id)
        {
            lock (syncRoot)
            {
                return claims.FirstOrDefault(claim => claim.Id == id);
            }
        }

        public IReadOnlyCollection<Claim> List()
        {
            lock(syncRoot)
            {
                return claims.ToArray();
            }
        }
    }
}
