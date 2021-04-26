using LuduStack.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

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