using HealthCare.Claims.Modules.Reporting.Application;
using HealthCare.Claims.Modules.Reporting.Domain;

namespace HealthCare.Claims.Modules.Reporting.Infrastructure
{
    public sealed class InMemoryReportingProjectionRepository : IReportingProjectionRepository
    {
        private readonly OperationalProjection projection = new();
        private readonly List<RecentBusinessEvent> recentEvents =
            [
                new(
                "ProjectionInitialized",
                "reporting",
                "Reporting read model initialized and waiting for business events.",
                DateTimeOffset.UtcNow)
            ];
        private readonly object syncRoot = new();
        public void AddRecentEvent(RecentBusinessEvent recentEvent)
        {
            lock (syncRoot)
            {
                recentEvents.Add(recentEvent);
            }
        }

        public OperationalProjection GetProjection()
        {
            lock (syncRoot)
            {
                return projection;
            }
        }

        public IReadOnlyCollection<RecentBusinessEvent> ListRecentEvents()
        {
            lock (syncRoot) 
            {
                return recentEvents
                    .OrderByDescending(recentEvent => recentEvent.OccurredOn)
                    .Take(20)
                    .ToArray();
            }
        }
    }
}
