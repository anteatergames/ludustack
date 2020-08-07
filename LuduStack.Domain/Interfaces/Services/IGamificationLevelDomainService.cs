using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGamificationLevelDomainService : IDomainService<GamificationLevel>
    {
        Task<GamificationLevel> GenerateNew(Guid userId);
        Task<bool> ValidateXp(int xpToAchieve, Guid id);
    }
}