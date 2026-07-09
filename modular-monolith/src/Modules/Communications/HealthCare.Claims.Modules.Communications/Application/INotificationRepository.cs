using HealthCare.Claims.Modules.Communications.Domain;

namespace HealthCare.Claims.Modules.Communications.Application
{
    public interface INotificationRepository
    {
        IReadOnlyCollection<NotificationMessage> List();
        void Add(NotificationMessage notification);
    }
}
