using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;

namespace LuduStack.Domain.Services
{
    public class NotificationDomainService : BaseDomainMongoService<Notification, INotificationRepository>, INotificationDomainService
    {
        public NotificationDomainService(INotificationRepository repository) : base(repository)
        {
        }
    }
}