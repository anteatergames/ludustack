using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;

namespace LuduStack.Domain.Services
{
    public class NotificationDomainService : INotificationDomainService
    {
        protected readonly INotificationRepository notificationRepository;

        public NotificationDomainService(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }
    }
}