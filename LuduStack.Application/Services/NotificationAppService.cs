using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Notification;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class NotificationAppService : BaseAppService, INotificationAppService
    {
        public NotificationAppService(IBaseAppServiceCommon baseAppServiceCommon) : base(baseAppServiceCommon)
        {
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, NotificationItemViewModel viewModel)
        {
            try
            {
                Notification model;

                Notification existing = await mediator.Query<GetNotificationByIdQuery, Notification>(new GetNotificationByIdQuery(viewModel.Id));

                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Notification>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveNotificationCommand(currentUserId, model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                await mediator.SendCommand(new DeleteNotificationCommand(currentUserId, id));

                return new OperationResultVo(true, "That Notification is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultListVo<NotificationItemViewModel>> GetByUserId(Guid userId, int count)
        {
            IEnumerable<Notification> userNotifications = await mediator.Query<GetNotificationByUserIdQuery, IEnumerable<Notification>>(new GetNotificationByUserIdQuery(userId));
            List<Notification> notifications = userNotifications.OrderByDescending(x => x.CreateDate).Take(count).ToList();

            IEnumerable<NotificationItemViewModel> vms = mapper.Map<IEnumerable<NotificationItemViewModel>>(notifications);

            return new OperationResultListVo<NotificationItemViewModel>(vms);
        }

        public async Task<OperationResultVo> Notify(Guid originUserId, string originUserName, Guid targetUserId, NotificationType notificationType, Guid targetId)
        {
            return await Notify(originUserId, originUserName, targetUserId, notificationType, targetId, null);
        }

        public async Task<OperationResultVo> Notify(Guid originUserId, string originUserName, Guid targetUserId, NotificationType notificationType, Guid targetId, string targetName)
        {
            NotificationItemViewModel vm = new NotificationItemViewModel
            {
                UserId = targetUserId,
                Type = notificationType,
                OriginId = originUserId,
                OriginName = originUserName,
                TargetId = targetId,
                TargetName = targetName
            };

            return await Save(originUserId, vm);
        }

        public async Task<OperationResultVo> MarkAsRead(Guid id)
        {
            try
            {
                Notification notification = await mediator.Query<GetNotificationByIdQuery, Notification>(new GetNotificationByIdQuery(id));

                if (notification != null)
                {
                    notification.IsRead = true;

                    CommandResult result = await mediator.SendCommand(new SaveNotificationCommand(notification.UserId, notification));

                    if (!result.Validation.IsValid)
                    {
                        string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                        return new OperationResultVo(false, message);
                    }
                }

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }
    }
}