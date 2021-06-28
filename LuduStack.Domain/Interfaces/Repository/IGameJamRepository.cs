using LuduStack.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGameJamRepository : IRepository<GameJam>
    {
        Task<IQueryable<GameJamListItem>> GetList();
    }
}