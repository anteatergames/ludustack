using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGameJamDomainService
    {
        void CheckTeamMembers(GameJamEntry entry, IEnumerable<Guid> userIds);

        void CheckTeamMembers(GameJamEntry obj);
    }
}