namespace HealthCare.Claims.Modules.Audit.Application
{
    public sealed class AuditApplicationService(IAuditEntryRepository auditEntries)
    {
        public IReadOnlyCollection<AuditEntrySummary> List(string? module = null, string? eventName = null)
        {
            var query = auditEntries.List().AsEnumerable();
            if(!string.IsNullOrWhiteSpace(module))
            {
                query = query.Where(entry => string.Equals(entry.Module, module, StringComparison.OrdinalIgnoreCase));
            }
            if(!string.IsNullOrWhiteSpace(eventName))
            {
                query = query.Where(entry => string.Equals(entry.EventName, eventName, StringComparison.OrdinalIgnoreCase));
            }
            return query
                   .OrderByDescending(entry => entry.OccurredOn)
                   .Select(AuditEntrySummary.FromEntry)
                   .ToArray();
        }
    }
}
