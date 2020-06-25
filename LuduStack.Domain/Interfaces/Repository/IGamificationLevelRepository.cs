using LuduStack.Domain.Models;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGamificationLevelRepository : IRepository<GamificationLevel>
    {
        Task<GamificationLevel> GetByNumber(int levelNumber);
    }
}