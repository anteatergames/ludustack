using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface INotificationAppService : ICrudAppService<NotificationItemViewModel>
    {
        OperationResultListVo<NotificationItemViewModel> GetByUserId(Guid userId, int count);

        OperationResultVo Notify(Guid currentUserId, Guid targetUserId, NotificationType notificationType, Guid targetId, string text, string url);

        OperationResultVo MarkAsRead(Guid id);
    }
}