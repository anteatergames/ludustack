using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface INotificationAppService : ICrudAppService<NotificationItemViewModel>
    {
        OperationResultListVo<NotificationItemViewModel> GetByUserId(Guid userId, int count);

        OperationResultVo Notify(Guid currentUserId, string originName, Guid targetUserId, NotificationType notificationType, Guid targetId);

        OperationResultVo Notify(Guid currentUserId, string originName, Guid targetUserId, NotificationType notificationType, Guid targetId, string targetName);

        OperationResultVo MarkAsRead(Guid id);
    }
}