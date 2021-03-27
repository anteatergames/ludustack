using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace LuduStack.Application.Interfaces
{
    public interface INotificationAppService
    {
        Task<OperationResultVo<int>> Count(Guid currentUserId);

        Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId);

        Task<OperationResultVo<NotificationItemViewModel>> GetById(Guid currentUserId, Guid id);

        Task<OperationResultVo> Remove(Guid currentUserId, Guid id);

        Task<OperationResultVo<Guid>> Save(Guid currentUserId, NotificationItemViewModel viewModel);

        OperationResultListVo<NotificationItemViewModel> GetByUserId(Guid userId, int count);

        Task<OperationResultVo> Notify(Guid originUserId, string originUserName, Guid targetUserId, NotificationType notificationType, Guid targetId);

        Task<OperationResultVo> Notify(Guid originUserId, string originUserName, Guid targetUserId, NotificationType notificationType, Guid targetId, string targetName);

        Task<OperationResultVo> MarkAsRead(Guid id);
    }
}