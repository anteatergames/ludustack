using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
