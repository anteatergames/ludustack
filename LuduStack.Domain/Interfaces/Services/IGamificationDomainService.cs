using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGamificationDomainService
    {
        int ProcessAction(Guid userId, PlatformAction action);

        Gamification GenerateNewGamification(Guid userId);
    }
}