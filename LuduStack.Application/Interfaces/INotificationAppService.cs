using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface INotificationAppService : ICrudAppService<NotificationItemViewModel>
    {
        OperationResultListVo<NotificationItemViewModel> GetByUserId(Guid userId, int count);

        Task<OperationResultVo> Notify(Guid currentUserId, string originName, Guid targetUserId, NotificationType notificationType, Guid targetId);

        Task<OperationResultVo> Notify(Guid currentUserId, string originName, Guid targetUserId, NotificationType notificationType, Guid targetId, string targetName);

        OperationResultVo MarkAsRead(Guid id);
    }
}