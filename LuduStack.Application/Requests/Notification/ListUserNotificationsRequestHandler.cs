using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Notification;
using LuduStack.Domain.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Application.Requests.Notification
{
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