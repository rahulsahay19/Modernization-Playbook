using HealthCare.Claims.Modules.Documents.Application;
using HealthCare.Claims.Modules.Documents.Domain;

namespace HealthCare.Claims.Modules.Documents.Infrastructure
{
    public sealed class InMemoryClaimDocumentRepository : IClaimDocumentRepository
    {
        private readonly List<ClaimDocument> documents =
            [
               new(
            Guid.Parse("50000000-0000-0000-0000-000000000001"),
            "CLM-202606270001",
            ClaimDocumentType.DischargeSummary,
            "discharge-summary-clm-202606270001.pdf",
            "local://claims/CLM-202606270001/discharge-summary.pdf",
            ClaimDocumentStatus.Verified,
            "Hospital discharge summary verified."),
        new(
            Guid.Parse("50000000-0000-0000-0000-000000000002"),
            "CLM-202606270001",
            ClaimDocumentType.FinalBill,
            "final-bill-clm-202606270001.pdf",
            "local://claims/CLM-202606270001/final-bill.pdf",
            ClaimDocumentStatus.Received,
            "Pending billing team verification.")
            ];
        private readonly object syncRoot = new();

        public IReadOnlyCollection<ClaimDocument> List()
        {
            lock(syncRoot)
            {
                return documents.ToArray();
            }
        }
        public ClaimDocument? GetById(Guid id)
        {
            lock (syncRoot)
            {
                return documents.FirstOrDefault(document => document.Id == id);
            }
        }

        public void Add(ClaimDocument document)
        {
            lock (syncRoot)
            {
                documents.Add(document);
            }
        }

    }
}
