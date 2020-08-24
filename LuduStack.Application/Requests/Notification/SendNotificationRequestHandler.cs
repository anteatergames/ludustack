using LuduStack.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Application.Requests.Notification
{
    public class SendNotificationRequestHandler : IRequestHandler<SendNotificationRequest>
    {
        private readonly INotificationAppService notificationAppService;

        public SendNotificationRequestHandler(INotificationAppService notificationAppService)
        {
            this.notificationAppService = notificationAppService;
        }

        public Task<Unit> Handle(SendNotificationRequest request, CancellationToken cancellationToken)
        {
            notificationAppService.MarkAsRead(request.NotificationId);

            return Task.FromResult(Unit.Value);
        }
    }
}
