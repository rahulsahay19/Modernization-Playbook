namespace HealthCare.Claims.Monolith.Models;

public enum ClaimStatus
{
    Draft,
    Submitted,
    PendingDocuments,
    UnderReview,
    Approved,
    Rejected,
    Paid
}

public enum DocumentStatus
{
    Uploaded,
    Verified,
    Rejected
}

public enum PaymentStatus
{
    Pending,
    Scheduled,
    Settled,
    Failed
}

public enum NotificationChannel
{
    Email,
    Sms
}

public enum NotificationStatus
{
    Queued,
    Sent,
    Failed
}

public sealed class Policy
{
    public required string PolicyNumber { get; set; }
    public required string InsuranceProviderName { get; set; }
    public required string PlanName { get; set; }
    public decimal AnnualLimit { get; set; }
    public decimal Deductible { get; set; }
    public bool IsActive { get; set; }
}

public sealed class Member
{
    public required string MemberId { get; set; }
    public required string FullName { get; set; }
    public required string PolicyNumber { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class Provider
{
    public required string ProviderId { get; set; }
    public required string Name { get; set; }
    public required string NetworkTier { get; set; }
    public string City { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class Claim
{
    public required string ClaimNumber { get; set; }
    public required string MemberId { get; set; }
    public required string ProviderId { get; set; }
    public DateOnly ServiceDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public ClaimStatus Status { get; set; }
    public List<ClaimLine> Lines { get; set; } = [];
    public List<string> DocumentIds { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class ClaimLine
{
    public required string Code { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
}

public sealed class ClaimDocument
{
    public required string DocumentId { get; set; }
    public required string ClaimNumber { get; set; }
    public required string FileName { get; set; }
    public required string DocumentType { get; set; }
    public required string StoragePath { get; set; }
    public DocumentStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}

public sealed class Payment
{
    public required string PaymentId { get; set; }
    public required string ClaimNumber { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string PaymentMode { get; set; } = "NEFT";
    public string? SettlementReference { get; set; }
    public string? FailureReason { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public DateOnly? SettledDate { get; set; }
}

public sealed class NotificationMessage
{
    public required string NotificationId { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
    public required string Recipient { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}

public sealed class AuditEntry
{
    public required string AuditId { get; set; }
    public required string EntityType { get; set; }
    public required string EntityId { get; set; }
    public required string Action { get; set; }
    public required string Description { get; set; }
    public required string PerformedBy { get; set; }
    public DateTimeOffset PerformedAt { get; set; }
}

public sealed record CreatePolicyRequest(
    string PolicyNumber,
    string InsuranceProviderName,
    string PlanName,
    decimal AnnualLimit,
    decimal Deductible);

public sealed record UpdatePolicyRequest(
    string InsuranceProviderName,
    string PlanName,
    decimal AnnualLimit,
    decimal Deductible,
    bool IsActive);

public sealed record CreateMemberRequest(
    string MemberId,
    string FullName,
    string PolicyNumber,
    DateOnly DateOfBirth,
    string Email,
    string MobileNumber);

public sealed record UpdateMemberRequest(
    string FullName,
    string PolicyNumber,
    DateOnly DateOfBirth,
    string Email,
    string MobileNumber,
    bool IsActive);

public sealed record CreateProviderRequest(
    string ProviderId,
    string Name,
    string NetworkTier,
    string City);

public sealed record UpdateProviderRequest(
    string Name,
    string NetworkTier,
    string City,
    bool IsActive);

public sealed record SubmitClaimRequest(
    string MemberId,
    string ProviderId,
    DateOnly ServiceDate,
    IReadOnlyCollection<ClaimLine> Lines);

public sealed record UploadDocumentRequest(
    string ClaimNumber,
    string FileName,
    string DocumentType);

public sealed record VerifyDocumentRequest(bool IsVerified, string? RejectionReason);

public sealed record ApproveClaimRequest(decimal ApprovedAmount);

public sealed record RejectClaimRequest(string Reason);

public sealed record SettlePaymentRequest(string SettlementReference, DateOnly SettledDate);

public sealed record FailPaymentRequest(string FailureReason);
