using HealthCare.Claims.Monolith.Models;
using HealthCare.Claims.Monolith.Repositories;

namespace HealthCare.Claims.Monolith.Services;

public sealed class PolicyService(IPolicyRepository policies, AuditService audit, IUnitOfWork unitOfWork)
{
    public IReadOnlyCollection<Policy> GetPolicies() => policies.GetAll();

    public Policy CreatePolicy(CreatePolicyRequest request)
    {
        if (policies.GetByNumber(request.PolicyNumber) is not null)
        {
            throw new InvalidOperationException($"Policy {request.PolicyNumber} already exists.");
        }

        var policy = new Policy
        {
            PolicyNumber = request.PolicyNumber,
            InsuranceProviderName = request.InsuranceProviderName,
            PlanName = request.PlanName,
            AnnualLimit = request.AnnualLimit,
            Deductible = request.Deductible,
            IsActive = true
        };

        policies.Add(policy);
        audit.Record("Policy", policy.PolicyNumber, "Created", $"Created policy {policy.PlanName}.");
        unitOfWork.SaveChanges();
        return policy;
    }

    public Policy UpdatePolicy(string policyNumber, UpdatePolicyRequest request)
    {
        var policy = policies.GetByNumber(policyNumber)
            ?? throw new InvalidOperationException($"Policy {policyNumber} was not found.");

        policy.InsuranceProviderName = request.InsuranceProviderName;
        policy.PlanName = request.PlanName;
        policy.AnnualLimit = request.AnnualLimit;
        policy.Deductible = request.Deductible;
        policy.IsActive = request.IsActive;

        audit.Record("Policy", policy.PolicyNumber, "Updated", $"Updated policy {policy.PolicyNumber}.");
        unitOfWork.SaveChanges();
        return policy;
    }

    public Policy DeactivatePolicy(string policyNumber)
    {
        var policy = policies.GetByNumber(policyNumber)
            ?? throw new InvalidOperationException($"Policy {policyNumber} was not found.");

        policy.IsActive = false;
        audit.Record("Policy", policy.PolicyNumber, "Deactivated", $"Deactivated policy {policy.PolicyNumber}.");
        unitOfWork.SaveChanges();
        return policy;
    }
}

public sealed class MemberService(
    IMemberRepository members,
    IPolicyRepository policies,
    AuditService audit,
    IUnitOfWork unitOfWork)
{
    public IReadOnlyCollection<Member> GetMembers() => members.GetAll();

    public Member CreateMember(CreateMemberRequest request)
    {
        if (members.GetById(request.MemberId) is not null)
        {
            throw new InvalidOperationException($"Member {request.MemberId} already exists.");
        }

        _ = policies.GetActiveByNumber(request.PolicyNumber)
            ?? throw new InvalidOperationException($"Active policy {request.PolicyNumber} was not found.");

        var member = new Member
        {
            MemberId = request.MemberId,
            FullName = request.FullName,
            PolicyNumber = request.PolicyNumber,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            MobileNumber = request.MobileNumber,
            IsActive = true
        };

        members.Add(member);
        audit.Record("Member", member.MemberId, "Created", $"Created member {member.FullName}.");
        unitOfWork.SaveChanges();
        return member;
    }

    public Member UpdateMember(string memberId, UpdateMemberRequest request)
    {
        var member = members.GetById(memberId)
            ?? throw new InvalidOperationException($"Member {memberId} was not found.");

        _ = policies.GetActiveByNumber(request.PolicyNumber)
            ?? throw new InvalidOperationException($"Active policy {request.PolicyNumber} was not found.");

        member.FullName = request.FullName;
        member.PolicyNumber = request.PolicyNumber;
        member.DateOfBirth = request.DateOfBirth;
        member.Email = request.Email;
        member.MobileNumber = request.MobileNumber;
        member.IsActive = request.IsActive;

        audit.Record("Member", member.MemberId, "Updated", $"Updated member {member.FullName}.");
        unitOfWork.SaveChanges();
        return member;
    }

    public Member DeactivateMember(string memberId)
    {
        var member = members.GetById(memberId)
            ?? throw new InvalidOperationException($"Member {memberId} was not found.");

        member.IsActive = false;
        audit.Record("Member", member.MemberId, "Deactivated", $"Deactivated member {member.FullName}.");
        unitOfWork.SaveChanges();
        return member;
    }
}

public sealed class ProviderService(IProviderRepository providers, AuditService audit, IUnitOfWork unitOfWork)
{
    public IReadOnlyCollection<Provider> GetProviders() => providers.GetAll();

    public Provider CreateProvider(CreateProviderRequest request)
    {
        if (providers.GetById(request.ProviderId) is not null)
        {
            throw new InvalidOperationException($"Provider {request.ProviderId} already exists.");
        }

        var provider = new Provider
        {
            ProviderId = request.ProviderId,
            Name = request.Name,
            NetworkTier = request.NetworkTier,
            City = request.City,
            IsActive = true
        };

        providers.Add(provider);
        audit.Record("Provider", provider.ProviderId, "Created", $"Created provider {provider.Name}.");
        unitOfWork.SaveChanges();
        return provider;
    }

    public Provider UpdateProvider(string providerId, UpdateProviderRequest request)
    {
        var provider = providers.GetById(providerId)
            ?? throw new InvalidOperationException($"Provider {providerId} was not found.");

        provider.Name = request.Name;
        provider.NetworkTier = request.NetworkTier;
        provider.City = request.City;
        provider.IsActive = request.IsActive;

        audit.Record("Provider", provider.ProviderId, "Updated", $"Updated provider {provider.Name}.");
        unitOfWork.SaveChanges();
        return provider;
    }

    public Provider DeactivateProvider(string providerId)
    {
        var provider = providers.GetById(providerId)
            ?? throw new InvalidOperationException($"Provider {providerId} was not found.");

        provider.IsActive = false;
        audit.Record("Provider", provider.ProviderId, "Deactivated", $"Deactivated provider {provider.Name}.");
        unitOfWork.SaveChanges();
        return provider;
    }
}

public sealed class ClaimsWorkflowService(
    IClaimRepository claims,
    IMemberRepository members,
    IPolicyRepository policies,
    IProviderRepository providers,
    IDocumentRepository documents,
    NotificationService notifications,
    AuditService audit,
    IUnitOfWork unitOfWork)
{
    public IReadOnlyCollection<Claim> GetClaims() => claims.GetAll();

    public Claim? GetClaim(string claimNumber) => claims.GetByNumber(claimNumber);

    public Claim SubmitClaim(SubmitClaimRequest request)
    {
        var member = members.GetById(request.MemberId)
            ?? throw new InvalidOperationException($"Member {request.MemberId} was not found.");

        if (!member.IsActive)
        {
            throw new InvalidOperationException($"Member {request.MemberId} is inactive.");
        }

        var policy = policies.GetActiveByNumber(member.PolicyNumber)
            ?? throw new InvalidOperationException($"Active policy for member {request.MemberId} was not found.");

        var provider = providers.GetActiveById(request.ProviderId)
            ?? throw new InvalidOperationException($"Active provider {request.ProviderId} was not found.");

        var requestedAmount = request.Lines.Sum(line => line.Amount);
        if (requestedAmount > policy.AnnualLimit)
        {
            throw new InvalidOperationException("Claim exceeds the policy annual limit.");
        }

        var claim = new Claim
        {
            ClaimNumber = $"CLM-{DateTime.UtcNow:HHmmssfff}",
            MemberId = member.MemberId,
            ProviderId = provider.ProviderId,
            ServiceDate = request.ServiceDate,
            RequestedAmount = requestedAmount,
            ApprovedAmount = 0,
            Status = ClaimStatus.PendingDocuments,
            CreatedAt = DateTimeOffset.UtcNow,
            Lines = request.Lines.ToList()
        };

        claims.Add(claim);
        notifications.Send(member.Email, NotificationChannel.Email, "Claim submitted", $"Claim {claim.ClaimNumber} has been submitted for {provider.Name}.");
        notifications.Send(member.MobileNumber, NotificationChannel.Sms, "Claim submitted", $"Claim {claim.ClaimNumber} submitted.");
        audit.Record("Claim", claim.ClaimNumber, "Submitted", $"Submitted claim for {FormatRupees(claim.RequestedAmount)}.");
        unitOfWork.SaveChanges();
        return claim;
    }

    public Claim ApproveClaim(string claimNumber, ApproveClaimRequest request)
    {
        var claim = GetRequiredClaim(claimNumber);
        var member = members.GetById(claim.MemberId)
            ?? throw new InvalidOperationException($"Member {claim.MemberId} was not found.");
        var uploadedDocuments = documents.GetByClaimNumber(claimNumber);

        if (uploadedDocuments.Count == 0 || uploadedDocuments.Any(document => document.Status != DocumentStatus.Verified))
        {
            claim.Status = ClaimStatus.PendingDocuments;
            throw new InvalidOperationException("Claim cannot be approved until all uploaded documents are verified.");
        }

        claim.ApprovedAmount = request.ApprovedAmount;
        claim.Status = ClaimStatus.Approved;
        notifications.Send(member.Email, NotificationChannel.Email, "Claim approved", $"Claim {claim.ClaimNumber} was approved for {FormatRupees(claim.ApprovedAmount)}.");
        audit.Record("Claim", claim.ClaimNumber, "Approved", $"Approved claim for {FormatRupees(claim.ApprovedAmount)}.");
        unitOfWork.SaveChanges();
        return claim;
    }

    public Claim RejectClaim(string claimNumber, RejectClaimRequest request)
    {
        var claim = GetRequiredClaim(claimNumber);
        var member = members.GetById(claim.MemberId)
            ?? throw new InvalidOperationException($"Member {claim.MemberId} was not found.");

        claim.Status = ClaimStatus.Rejected;
        notifications.Send(member.Email, NotificationChannel.Email, "Claim rejected", $"Claim {claim.ClaimNumber} was rejected. Reason: {request.Reason}");
        audit.Record("Claim", claim.ClaimNumber, "Rejected", request.Reason);
        unitOfWork.SaveChanges();
        return claim;
    }

    private Claim GetRequiredClaim(string claimNumber) =>
        claims.GetTrackedByNumber(claimNumber)
            ?? throw new InvalidOperationException($"Claim {claimNumber} was not found.");

    private static string FormatRupees(decimal amount) => $"Rs. {amount:N2}";
}

public sealed class DocumentService(
    IDocumentRepository documents,
    IClaimRepository claims,
    IMemberRepository members,
    NotificationService notifications,
    AuditService audit,
    IUnitOfWork unitOfWork)
{
    public IReadOnlyCollection<ClaimDocument> GetDocuments() => documents.GetAll();

    public ClaimDocument UploadDocument(UploadDocumentRequest request)
    {
        var claim = claims.GetTrackedByNumber(request.ClaimNumber)
            ?? throw new InvalidOperationException($"Claim {request.ClaimNumber} was not found.");

        var member = members.GetById(claim.MemberId)
            ?? throw new InvalidOperationException($"Member {claim.MemberId} was not found.");
        var document = new ClaimDocument
        {
            DocumentId = $"DOC-{DateTime.UtcNow:HHmmssfff}",
            ClaimNumber = request.ClaimNumber,
            FileName = request.FileName,
            DocumentType = request.DocumentType,
            StoragePath = $"/SimulatedDocumentStore/{request.ClaimNumber}/{request.FileName}",
            Status = DocumentStatus.Uploaded,
            UploadedAt = DateTimeOffset.UtcNow
        };

        documents.Add(document);
        claim.DocumentIds.Add(document.DocumentId);
        claim.Status = ClaimStatus.UnderReview;
        notifications.Send(member.Email, NotificationChannel.Email, "Document received", $"Document {document.FileName} was attached to claim {claim.ClaimNumber}.");
        audit.Record("Document", document.DocumentId, "Uploaded", $"Uploaded {document.DocumentType} for claim {claim.ClaimNumber}.");
        unitOfWork.SaveChanges();
        return document;
    }

    public ClaimDocument VerifyDocument(string documentId, VerifyDocumentRequest request)
    {
        var document = documents.GetTrackedById(documentId)
            ?? throw new InvalidOperationException($"Document {documentId} was not found.");

        document.Status = request.IsVerified ? DocumentStatus.Verified : DocumentStatus.Rejected;
        document.RejectionReason = request.IsVerified ? null : request.RejectionReason;

        audit.Record("Document", document.DocumentId, request.IsVerified ? "Verified" : "Rejected", $"Document {document.FileName} status changed to {document.Status}.");
        unitOfWork.SaveChanges();
        return document;
    }
}

public sealed class PaymentService(
    IPaymentRepository payments,
    IClaimRepository claims,
    IMemberRepository members,
    NotificationService notifications,
    AuditService audit,
    IUnitOfWork unitOfWork)
{
    public IReadOnlyCollection<Payment> GetPayments() => payments.GetAll();

    public Payment SchedulePayment(string claimNumber)
    {
        var claim = claims.GetTrackedByNumber(claimNumber)
            ?? throw new InvalidOperationException($"Claim {claimNumber} was not found.");

        if (claim.Status != ClaimStatus.Approved)
        {
            throw new InvalidOperationException("Only approved claims can be scheduled for payment.");
        }

        var member = members.GetById(claim.MemberId)
            ?? throw new InvalidOperationException($"Member {claim.MemberId} was not found.");
        var payment = new Payment
        {
            PaymentId = $"PAY-{DateTime.UtcNow:HHmmssfff}",
            ClaimNumber = claim.ClaimNumber,
            Amount = claim.ApprovedAmount,
            Status = PaymentStatus.Scheduled,
            PaymentMode = "NEFT",
            ScheduledDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
        };

        payments.Add(payment);
        notifications.Send(member.Email, NotificationChannel.Email, "Payment scheduled", $"Payment {payment.PaymentId} was scheduled for claim {claim.ClaimNumber}.");
        audit.Record("Payment", payment.PaymentId, "Scheduled", $"Scheduled payment for {FormatRupees(payment.Amount)}.");
        unitOfWork.SaveChanges();
        return payment;
    }

    public Payment SettlePayment(string paymentId, SettlePaymentRequest request)
    {
        var payment = payments.GetTrackedById(paymentId)
            ?? throw new InvalidOperationException($"Payment {paymentId} was not found.");
        var claim = claims.GetTrackedByNumber(payment.ClaimNumber)
            ?? throw new InvalidOperationException($"Claim {payment.ClaimNumber} was not found.");

        payment.Status = PaymentStatus.Settled;
        payment.SettlementReference = request.SettlementReference;
        payment.SettledDate = request.SettledDate;
        payment.FailureReason = null;
        claim.Status = ClaimStatus.Paid;

        audit.Record("Payment", payment.PaymentId, "Settled", $"Settled payment using reference {request.SettlementReference}.");
        unitOfWork.SaveChanges();
        return payment;
    }

    public Payment FailPayment(string paymentId, FailPaymentRequest request)
    {
        var payment = payments.GetTrackedById(paymentId)
            ?? throw new InvalidOperationException($"Payment {paymentId} was not found.");

        payment.Status = PaymentStatus.Failed;
        payment.FailureReason = request.FailureReason;
        audit.Record("Payment", payment.PaymentId, "Failed", request.FailureReason);
        unitOfWork.SaveChanges();
        return payment;
    }

    private static string FormatRupees(decimal amount) => $"Rs. {amount:N2}";
}

public sealed class NotificationService(INotificationRepository notifications)
{
    public IReadOnlyCollection<NotificationMessage> GetNotifications() => notifications.GetAll();

    public NotificationMessage Send(string recipient, NotificationChannel channel, string subject, string body)
    {
        var notification = new NotificationMessage
        {
            NotificationId = $"NOT-{DateTime.UtcNow:HHmmssfff}",
            Channel = channel,
            Status = NotificationStatus.Sent,
            Recipient = recipient,
            Subject = subject,
            Body = body,
            CreatedAt = DateTimeOffset.UtcNow,
            SentAt = DateTimeOffset.UtcNow
        };

        notifications.Add(notification);
        return notification;
    }
}

public sealed class AuditService(IAuditRepository auditEntries)
{
    public IReadOnlyCollection<AuditEntry> GetAuditEntries() => auditEntries.GetAll();

    public void Record(string entityType, string entityId, string action, string description)
    {
        auditEntries.Add(new AuditEntry
        {
            AuditId = $"AUD-{DateTime.UtcNow:HHmmssfff}",
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Description = description,
            PerformedBy = "legacy-system",
            PerformedAt = DateTimeOffset.UtcNow
        });
    }
}

public sealed class ReportingService(IReportingRepository reporting)
{
    public object GetOperationalDashboard(DateOnly? from = null, DateOnly? to = null)
    {
        var claims = reporting.GetClaimsSnapshot()
            .Where(claim => from is null || claim.ServiceDate >= from)
            .Where(claim => to is null || claim.ServiceDate <= to)
            .ToList();
        var payments = reporting.GetPaymentsSnapshot();
        var documents = reporting.GetDocumentsSnapshot();

        return new
        {
            TotalPolicies = reporting.CountPolicies(),
            TotalMembers = reporting.CountMembers(),
            TotalProviders = reporting.CountProviders(),
            TotalClaims = claims.Count,
            ClaimsByStatus = claims
                .GroupBy(claim => claim.Status)
                .Select(group => new { Status = group.Key.ToString(), Count = group.Count() })
                .ToList(),
            DocumentsByStatus = documents
                .GroupBy(document => document.Status)
                .Select(group => new { Status = group.Key.ToString(), Count = group.Count() })
                .ToList(),
            PaymentsByStatus = payments
                .GroupBy(payment => payment.Status)
                .Select(group => new { Status = group.Key.ToString(), Count = group.Count() })
                .ToList(),
            RequestedAmount = claims.Sum(claim => claim.RequestedAmount),
            ApprovedAmount = claims.Sum(claim => claim.ApprovedAmount),
            SettledAmount = payments
                .Where(payment => payment.Status == PaymentStatus.Settled)
                .Sum(payment => payment.Amount),
            PendingDocuments = claims.Count(claim => claim.Status == ClaimStatus.PendingDocuments),
            UploadedDocuments = reporting.CountDocuments(),
            ScheduledPayments = reporting.CountScheduledPayments(),
            NotificationsSent = reporting.CountNotifications()
        };
    }
}
