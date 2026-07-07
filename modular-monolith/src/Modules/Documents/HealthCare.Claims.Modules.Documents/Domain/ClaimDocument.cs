namespace HealthCare.Claims.Modules.Documents.Domain
{
    public sealed class ClaimDocument
    {
        public ClaimDocument(
            Guid id,
            string claimNumber,
            ClaimDocumentType documentType,
            string fileName,
            string storageReference,
            ClaimDocumentStatus status,
            string? notes = null)
        {
            Id = id;
            ClaimNumber = claimNumber;
            DocumentType = documentType;
            FileName = fileName;
            StorageReference = storageReference;
            Status = status;
            Notes = notes;
            ReceivedOn = DateTime.UtcNow;
        }
        public Guid Id { get; }
        public string ClaimNumber { get; }
        public ClaimDocumentType DocumentType { get; }
        public string FileName { get; }
        public string StorageReference { get; }
        public ClaimDocumentStatus Status { get; private set; }
        public string? Notes { get; private set; }

        public DateTimeOffset ReceivedOn { get; }
        public void Verify(string? notes)
        {
            Status = ClaimDocumentStatus.Verified;
            Notes = notes;
        }

        public void Reject(string notes)
        {
            Status = ClaimDocumentStatus.Rejected;
            Notes = notes;
        }
    }
}
