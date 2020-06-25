using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<bool> Follow(Guid userId, Guid gameId);

        Task<bool> Unfollow(Guid userId, Guid gameId);

        Task<bool> Like(Guid userId, Guid gameId);

        Task<bool> Unlike(Guid userId, Guid gameId);

        Task<int> CountFollowers(Guid gameId);

        Task<int> CountLikes(Guid gameId);
    }
}