using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Application.Interfaces
{
    public interface IPollAppService
    {
        OperationResultVo PollVote(Guid currentUserId, Guid pollOptionId);
    }
}