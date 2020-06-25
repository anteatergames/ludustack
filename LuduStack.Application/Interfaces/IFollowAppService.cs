using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IFollowAppService
    {
        OperationResultVo GameFollow(Guid currentUserId, Guid gameId);

        OperationResultVo GameUnfollow(Guid currentUserId, Guid gameId);

        OperationResultVo UserFollow(Guid currentUserId, Guid followUserId);

        OperationResultVo UserUnfollow(Guid currentUserId, Guid followUserId);
    }
}