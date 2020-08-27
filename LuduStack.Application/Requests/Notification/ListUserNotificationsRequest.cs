using LuduStack.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
