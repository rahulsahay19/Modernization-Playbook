using HealthCare.Claims.Monolith.Data;
using HealthCare.Claims.Monolith.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Claims.Monolith.Repositories;

public sealed class EfUnitOfWork(HealthCareClaimsDbContext database) : IUnitOfWork
{
    public void SaveChanges() => database.SaveChanges();
}

public sealed class PolicyRepository(HealthCareClaimsDbContext database) : IPolicyRepository
{
    public IReadOnlyCollection<Policy> GetAll() =>
        database.Policies.AsNoTracking().ToList();

    public Policy? GetByNumber(string policyNumber) =>
        database.Policies.SingleOrDefault(policy => policy.PolicyNumber == policyNumber);

    public Policy? GetActiveByNumber(string policyNumber) =>
        database.Policies.SingleOrDefault(policy => policy.PolicyNumber == policyNumber && policy.IsActive);

    public void Add(Policy policy) => database.Policies.Add(policy);
}

public sealed class MemberRepository(HealthCareClaimsDbContext database) : IMemberRepository
{
    public IReadOnlyCollection<Member> GetAll() =>
        database.Members.AsNoTracking().ToList();

    public Member? GetById(string memberId) =>
        database.Members.SingleOrDefault(member => member.MemberId == memberId);

    public void Add(Member member) => database.Members.Add(member);
}

public sealed class ProviderRepository(HealthCareClaimsDbContext database) : IProviderRepository
{
    public IReadOnlyCollection<Provider> GetAll() =>
        database.Providers.AsNoTracking().ToList();

    public Provider? GetById(string providerId) =>
        database.Providers.SingleOrDefault(provider => provider.ProviderId == providerId);

    public Provider? GetActiveById(string providerId) =>
        database.Providers.SingleOrDefault(provider => provider.ProviderId == providerId && provider.IsActive);

    public void Add(Provider provider) => database.Providers.Add(provider);
}

public sealed class ClaimRepository(HealthCareClaimsDbContext database) : IClaimRepository
{
    public IReadOnlyCollection<Claim> GetAll() =>
        database.Claims
            .Include(claim => claim.Lines)
            .AsNoTracking()
            .ToList();

    public Claim? GetByNumber(string claimNumber) =>
        database.Claims
            .Include(claim => claim.Lines)
            .AsNoTracking()
            .SingleOrDefault(claim => claim.ClaimNumber == claimNumber);

    public Claim? GetTrackedByNumber(string claimNumber) =>
        database.Claims
            .Include(claim => claim.Lines)
            .SingleOrDefault(claim => claim.ClaimNumber == claimNumber);

    public void Add(Claim claim) => database.Claims.Add(claim);
}

public sealed class DocumentRepository(HealthCareClaimsDbContext database) : IDocumentRepository
{
    public IReadOnlyCollection<ClaimDocument> GetAll() =>
        database.Documents.AsNoTracking().ToList();

    public IReadOnlyCollection<ClaimDocument> GetByClaimNumber(string claimNumber) =>
        database.Documents
            .Where(document => document.ClaimNumber == claimNumber)
            .AsNoTracking()
            .ToList();

    public ClaimDocument? GetTrackedById(string documentId) =>
        database.Documents.SingleOrDefault(document => document.DocumentId == documentId);

    public void Add(ClaimDocument document) => database.Documents.Add(document);
}

public sealed class PaymentRepository(HealthCareClaimsDbContext database) : IPaymentRepository
{
    public IReadOnlyCollection<Payment> GetAll() =>
        database.Payments.AsNoTracking().ToList();

    public Payment? GetTrackedById(string paymentId) =>
        database.Payments.SingleOrDefault(payment => payment.PaymentId == paymentId);

    public int CountScheduled() =>
        database.Payments.Count(payment => payment.Status == PaymentStatus.Scheduled);

    public void Add(Payment payment) => database.Payments.Add(payment);
}

public sealed class NotificationRepository(HealthCareClaimsDbContext database) : INotificationRepository
{
    public IReadOnlyCollection<NotificationMessage> GetAll() =>
        database.Notifications.AsNoTracking().ToList();

    public int Count() => database.Notifications.Count();

    public void Add(NotificationMessage notification) => database.Notifications.Add(notification);
}

public sealed class AuditRepository(HealthCareClaimsDbContext database) : IAuditRepository
{
    public IReadOnlyCollection<AuditEntry> GetAll() =>
        database.AuditEntries
            .AsNoTracking()
            .ToList()
            .OrderByDescending(audit => audit.PerformedAt)
            .ToList();

    public void Add(AuditEntry entry) => database.AuditEntries.Add(entry);
}

public sealed class ReportingRepository(HealthCareClaimsDbContext database) : IReportingRepository
{
    public int CountPolicies() => database.Policies.Count();

    public int CountMembers() => database.Members.Count();

    public int CountProviders() => database.Providers.Count();

    public IReadOnlyCollection<Claim> GetClaimsSnapshot() =>
        database.Claims.AsNoTracking().ToList();

    public int CountDocuments() => database.Documents.Count();

    public int CountScheduledPayments() =>
        database.Payments.Count(payment => payment.Status == PaymentStatus.Scheduled);

    public int CountNotifications() => database.Notifications.Count();

    public IReadOnlyCollection<Payment> GetPaymentsSnapshot() =>
        database.Payments.AsNoTracking().ToList();

    public IReadOnlyCollection<ClaimDocument> GetDocumentsSnapshot() =>
        database.Documents.AsNoTracking().ToList();
}
