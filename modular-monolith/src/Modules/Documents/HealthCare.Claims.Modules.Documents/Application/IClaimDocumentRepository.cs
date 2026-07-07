using HealthCare.Claims.Modules.Documents.Domain;

namespace HealthCare.Claims.Modules.Documents.Application
{
    public interface IClaimDocumentRepository
    {
        IReadOnlyCollection<ClaimDocument> List();
        ClaimDocument? GetById(Guid id);
        void Add(ClaimDocument claimDocument);
    }
}
