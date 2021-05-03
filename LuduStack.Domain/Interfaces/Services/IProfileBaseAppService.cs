using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IProfileBaseAppService
    {
        void SetProfileCache(Guid userId, UserProfileEssentialVo value);

        OperationResultVo GetCountries(Guid currentUserId);
    }
}