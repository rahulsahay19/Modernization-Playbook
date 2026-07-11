using HealthCare.Claims.Modules.Reporting.Domain;

namespace HealthCare.Claims.Modules.Reporting.Application
{
    public interface IReportingProjectionRepository
    {
        OperationalProjection GetProjection();
        IReadOnlyCollection<RecentBusinessEvent> ListRecentEvents();
        void AddRecentEvent(RecentBusinessEvent recentEvent);
    }
}
