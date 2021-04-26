using LuduStack.Application.Interfaces;
using LuduStack.Domain.ValueObjects;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

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

    public class DeleteNotificationRequestHandler : IRequestHandler<DeleteNotificationRequest, OperationResultVo>
    {
        private readonly INotificationAppService notificationAppService;

        public DeleteNotificationRequestHandler(INotificationAppService notificationAppService)
        {
            this.notificationAppService = notificationAppService;
        }

        public async Task<OperationResultVo> Handle(DeleteNotificationRequest request, CancellationToken cancellationToken)
        {
            OperationResultVo result = await notificationAppService.Remove(request.CurrentUserId, request.Id);

            return result;
        }
    }
}