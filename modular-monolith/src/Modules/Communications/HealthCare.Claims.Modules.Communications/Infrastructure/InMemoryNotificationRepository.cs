using HealthCare.Claims.Modules.Communications.Application;
using HealthCare.Claims.Modules.Communications.Domain;

namespace HealthCare.Claims.Modules.Communications.Infrastructure
{
    public sealed class InMemoryNotificationRepository : INotificationRepository
    {
        private readonly List<NotificationMessage> notifications =
        [
             new(
            Guid.Parse("80000000-0000-0000-0000-000000000001"),
            "operations@healthcare.local",
            NotificationChannel.Email,
            "Notification module initialized",
            "Communications module is ready to process in-process integration events.",
            "ModuleInitialized",
            NotificationStatus.Sent,
            DateTimeOffset.UtcNow)
        ];
        private readonly Object syncRoot = new();
        public void Add(NotificationMessage notification)
        {
            lock (syncRoot)
            {
                notifications.Add(notification);
            }
            
        }

        public IReadOnlyCollection<NotificationMessage> List()
        {
            lock (syncRoot)
            {
                return notifications.ToArray();
            }
        }
    }
}
