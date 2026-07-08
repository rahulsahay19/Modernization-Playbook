using HealthCare.Claims.Modules.Audit.Domain;

namespace HealthCare.Claims.Modules.Audit.Application
{
    public interface IAuditEntryRepository
    {
        IReadOnlyCollection<AuditEntry> List();
        void Add(AuditEntry entry);
    }
}
