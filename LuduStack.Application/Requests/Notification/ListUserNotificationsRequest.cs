using LuduStack.Domain.ValueObjects;
using MediatR;
using System;

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
}