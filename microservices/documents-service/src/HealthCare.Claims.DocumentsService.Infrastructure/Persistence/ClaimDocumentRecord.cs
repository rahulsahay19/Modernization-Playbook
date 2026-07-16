using HealthCare.Claims.DocumentsService.Domain.Enums;

namespace HealthCare.Claims.DocumentsService.Infrastructure.Persistence
{
    public sealed class ClaimDocumentRecord
    {
        public Guid Id { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public ClaimDocumentType DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string StorageReference { get; set; } = string.Empty;
        public ClaimDocumentStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset ReceivedOn { get; set; }
    }
}
