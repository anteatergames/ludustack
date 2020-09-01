using LuduStack.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Application.Requests.Notification
{
    public class DeleteNotificationRequest : IRequest<OperationResultVo>
    {
        public Guid CurrentUserId { get; }

        public Guid Id { get; }

        public DeleteNotificationRequest(Guid currentUserId, Guid id)
        {
            CurrentUserId = currentUserId;
            Id = id;
        }
    }
}
