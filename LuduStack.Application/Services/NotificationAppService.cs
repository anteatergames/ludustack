﻿using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class NotificationAppService : BaseAppService, INotificationAppService
    {
        private readonly INotificationDomainService notificationDomainService;

        public NotificationAppService(IBaseAppServiceCommon baseAppServiceCommon, INotificationDomainService notificationDomainService) : base(baseAppServiceCommon)
        {
            this.notificationDomainService = notificationDomainService;
        }

        #region ICrudAppService

        public Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultVo<int>(string.Empty));
        }

        public Task<OperationResultListVo<NotificationItemViewModel>> GetAll(Guid currentUserId)
        {
            return Task.FromResult(new OperationResultListVo<NotificationItemViewModel>(string.Empty));
        }

        public async Task<OperationResultVo> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = notificationDomainService.GetAllIds();

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        Task<OperationResultVo<NotificationItemViewModel>> ICrudAppService<NotificationItemViewModel>.GetById(Guid currentUserId, Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, NotificationItemViewModel viewModel)
        {
            try
            {
                Notification model;

                Notification existing = notificationDomainService.GetById(viewModel.Id);

                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<Notification>(viewModel);
                }

                if (viewModel.Id == Guid.Empty)
                {
                    notificationDomainService.Add(model);
                    viewModel.Id = model.Id;
                }
                else
                {
                    notificationDomainService.Update(model);
                }

                await unitOfWork.Commit();

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
                notificationDomainService.Remove(id);

                await unitOfWork.Commit();

                return new OperationResultVo(true, "That Notification is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        #endregion ICrudAppService

        public OperationResultListVo<NotificationItemViewModel> GetByUserId(Guid userId, int count)
        {
            List<Notification> notifications = notificationDomainService.GetByUserId(userId).OrderByDescending(x => x.CreateDate).Take(count).ToList();

            var vms = mapper.Map<IEnumerable<NotificationItemViewModel>>(notifications);

            return new OperationResultListVo<NotificationItemViewModel>(vms);
        }

        OperationResultVo<NotificationItemViewModel> GetById(Guid currentUserId, Guid id)
        {
            throw new NotImplementedException();
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

        public OperationResultVo MarkAsRead(Guid id)
        {
            try
            {
                Notification notification = notificationDomainService.GetById(id);

                if (notification != null)
                {
                    notification.IsRead = true;
                    notificationDomainService.Update(notification);

                    unitOfWork.Commit();
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