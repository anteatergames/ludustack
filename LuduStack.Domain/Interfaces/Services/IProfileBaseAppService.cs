using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IProfileBaseAppService
    {
        void SetProfileCache(Guid userId, UserProfile value);

        OperationResultVo GetCountries(Guid currentUserId);
    }
}