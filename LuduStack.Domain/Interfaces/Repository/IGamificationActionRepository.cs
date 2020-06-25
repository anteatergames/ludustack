using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGamificationActionRepository : IRepository<GamificationAction>
    {
        Task<GamificationAction> GetByAction(PlatformAction action);
    }
}