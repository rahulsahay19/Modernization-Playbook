using HealthCare.Claims.Monolith.Models;

namespace HealthCare.Claims.Monolith.Repositories;

public interface IUnitOfWork
{
    void SaveChanges();
}

public interface IPolicyRepository
{
    IReadOnlyCollection<Policy> GetAll();
    Policy? GetByNumber(string policyNumber);
    Policy? GetActiveByNumber(string policyNumber);
    void Add(Policy policy);
}

public interface IMemberRepository
{
    IReadOnlyCollection<Member> GetAll();
    Member? GetById(string memberId);
    void Add(Member member);
}

public interface IProviderRepository
{
    IReadOnlyCollection<Provider> GetAll();
    Provider? GetById(string providerId);
    Provider? GetActiveById(string providerId);
    void Add(Provider provider);
}

public interface IClaimRepository
{
    IReadOnlyCollection<Claim> GetAll();
    Claim? GetByNumber(string claimNumber);
    Claim? GetTrackedByNumber(string claimNumber);
    void Add(Claim claim);
}

public interface IDocumentRepository
{
    IReadOnlyCollection<ClaimDocument> GetAll();
    IReadOnlyCollection<ClaimDocument> GetByClaimNumber(string claimNumber);
    ClaimDocument? GetTrackedById(string documentId);
    void Add(ClaimDocument document);
}

public interface IPaymentRepository
{
    IReadOnlyCollection<Payment> GetAll();
    Payment? GetTrackedById(string paymentId);
    int CountScheduled();
    void Add(Payment payment);
}

public interface INotificationRepository
{
    IReadOnlyCollection<NotificationMessage> GetAll();
    int Count();
    void Add(NotificationMessage notification);
}

public interface IAuditRepository
{
    IReadOnlyCollection<AuditEntry> GetAll();
    void Add(AuditEntry entry);
}

public interface IReportingRepository
{
    int CountPolicies();
    int CountMembers();
    int CountProviders();
    IReadOnlyCollection<Claim> GetClaimsSnapshot();
    int CountDocuments();
    int CountScheduledPayments();
    int CountNotifications();
    IReadOnlyCollection<Payment> GetPaymentsSnapshot();
    IReadOnlyCollection<ClaimDocument> GetDocumentsSnapshot();
}
