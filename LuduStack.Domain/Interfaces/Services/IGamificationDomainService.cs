using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGamificationDomainService
    {
        int ProcessAction(Guid userId, PlatformAction action);
    }
}