using HealthCare.Claims.Modules.Audit.Application;
using HealthCare.Claims.Modules.Audit.Domain;

namespace HealthCare.Claims.Modules.Audit.Infrastructure
{
    public sealed class InMemoryAuditEntryRepository : IAuditEntryRepository
    {
        private readonly List<AuditEntry> auditEntries = 
            [
            new(
                 Guid.Parse("70000000-0000-0000-0000-000000000001"),
                "Platform",
                "ModuleCatalogInitialized",
                "modular-monolith",
                "Modular monolith host initialized with all registered modules.",
                DateTimeOffset.UtcNow)
            ];
        private readonly object syncRoot = new();
        public void Add(AuditEntry entry)
        {
            lock (syncRoot)
            {
                auditEntries.Add(entry);
            }
        }

        public IReadOnlyCollection<AuditEntry> List()
        {
            lock (syncRoot) 
            {
                return auditEntries.ToArray();
            }
        }
    }
}
