using LuduStack.Application.Interfaces;
using LuduStack.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Application.Requests.Notification
{
    public class DeleteNotificationRequestHandler : IRequestHandler<DeleteNotificationRequest, OperationResultVo>
    {
        private INotificationAppService notificationAppService;

        public DeleteNotificationRequestHandler(INotificationAppService notificationAppService)
        {
            this.notificationAppService = notificationAppService;
        }

        public async Task<OperationResultVo> Handle(DeleteNotificationRequest request, CancellationToken cancellationToken)
        {
            var result = notificationAppService.Remove(request.CurrentUserId, request.Id);

            return result;
        }
    }
}
