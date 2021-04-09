using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;

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