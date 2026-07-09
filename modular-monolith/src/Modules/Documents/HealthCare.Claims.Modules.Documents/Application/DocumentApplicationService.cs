using HealthCare.Claims.ModularMonolith.BuildingBlocks.Claims;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events;
using HealthCare.Claims.ModularMonolith.BuildingBlocks.Events.BusinessEvents;
using HealthCare.Claims.Modules.Documents.Domain;

namespace HealthCare.Claims.Modules.Documents.Application
{
    public sealed class DocumentApplicationService(
        IClaimDocumentRepository documents,
        IClaimReferenceReader claims,
        IEventBus eventBus)
    {
        public IReadOnlyCollection<ClaimDocumentSummary> List(string? claimNumber = null)
        {
            var query = documents.List().AsEnumerable();
            if(!string.IsNullOrWhiteSpace(claimNumber))
            {
                query = query.Where(document => 
                        string.Equals(document.ClaimNumber, claimNumber, StringComparison.OrdinalIgnoreCase));
            }
            return query
                   .OrderByDescending(document => document.ReceivedOn)
                   .Select(ClaimDocumentSummary.FromDocument)
                   .ToArray();
        }

        public ClaimDocumentSummary? GetById(Guid id)
        {
            var document = documents.GetById(id);
            return document is null ? null : ClaimDocumentSummary.FromDocument(document);
        }

        public ClaimDocumentSummary Register(RegisterClaimDocumentRequest request) 
        {
            _ = claims.GetByClaimNumber(request.ClaimNumber) ?? throw new InvalidOperationException("Claim was not found");
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                throw new InvalidOperationException("File name is required.");
            }
            if(string.IsNullOrWhiteSpace(request.StorageReference))
            {
                throw new InvalidOperationException("Storage reference is required.");
            }

            var document = new ClaimDocument(
                Guid.NewGuid(),
                request.ClaimNumber,
                request.DocumentType,
                request.FileName,
                request.StorageReference,
                ClaimDocumentStatus.Received,
                request.Notes);
            documents.Add(document);
            return ClaimDocumentSummary.FromDocument(document);
        }

        public ClaimDocumentSummary? Verify(Guid id, DocumentDecisionRequest request)
        {
            var document = documents.GetById(id);
            if(document is null)
            {
                return null;
            }

            document.Verify(request.Notes);
            eventBus.Publish(new DocumentVerifiedEvent(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                document.ClaimNumber,
                document.DocumentType.ToString(),
                document.FileName));
            return ClaimDocumentSummary.FromDocument(document);
        }

        public ClaimDocumentSummary? Reject(Guid id, DocumentDecisionRequest request)
        {
            var document = documents.GetById(id);
            if (document is null)
            {
                return null;
            }
            if(string.IsNullOrWhiteSpace(request.Notes))
            {
                throw new InvalidOperationException("Rejection notes are required");
            }

            document.Reject(request.Notes);
            return ClaimDocumentSummary.FromDocument(document);
        }
    }
}
