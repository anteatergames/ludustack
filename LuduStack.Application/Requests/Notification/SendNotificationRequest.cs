using MediatR;
using System;

namespace LuduStack.Application.Requests.Notification
{
    public class SendNotificationRequest : IRequest
    {
        public Guid NotificationId { get; }

        public SendNotificationRequest(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}