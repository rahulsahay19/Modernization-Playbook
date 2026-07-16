using HealthCare.Claims.DocumentsService.Domain.Entities;

namespace HealthCare.Claims.DocumentsService.Application.Abstractions;

public interface IClaimDocumentRepository
{
    IReadOnlyCollection<ClaimDocument> List(string? claimNumber = null);

    ClaimDocument? GetById(Guid id);

    void Add(ClaimDocument document);

    void Update(ClaimDocument document);
}
