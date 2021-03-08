﻿using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.ValueObjects;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Application.Requests.Notification
{
    public class ListUserNotificationsRequest : IRequest<OperationResultVo>
    {
        public Guid UserId { get; }

        public int Quantity { get; set; }

        public ListUserNotificationsRequest(Guid userId)
        {
            UserId = userId;
        }

        public ListUserNotificationsRequest(Guid userId, int quantity)
        {
            UserId = userId;
            Quantity = quantity;
        }
    }

    public class ListUserNotificationsRequestHandler : IRequestHandler<ListUserNotificationsRequest, OperationResultVo>
    {
        private INotificationAppService notificationAppService;

        public ListUserNotificationsRequestHandler(INotificationAppService notificationAppService)
        {
            this.notificationAppService = notificationAppService;
        }

        public async Task<OperationResultVo> Handle(ListUserNotificationsRequest request, CancellationToken cancellationToken)
        {
            if (request.Quantity == 0)
            {
                request.Quantity = 10;
            }

            OperationResultListVo<NotificationItemViewModel> result = notificationAppService.GetByUserId(request.UserId, request.Quantity);

            return result;
        }
    }
}