using LuduStack.Domain.ValueObjects;
using MediatR;
using System;

namespace LuduStack.Application.Requests.User
{
    public class DeleteUserFromPlatformRequest : IRequest<OperationResultVo>
    {
        public Guid CurrentUserId { get; }
        public Guid UserId { get; }

        public DeleteUserFromPlatformRequest(Guid currentUserId, Guid userId)
        {
            CurrentUserId = currentUserId;
            UserId = userId;
        }
    }
}