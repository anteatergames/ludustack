using LuduStack.Domain.Models;
using System;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<bool> FollowDirectly(Guid userId, Guid gameId);

        Task Follow(Guid userId, Guid gameId);

        Task<bool> UnfollowDirectly(Guid userId, Guid gameId);

        Task Unfollow(Guid userId, Guid gameId);

        Task<bool> LikeDirectly(Guid userId, Guid gameId);

        Task Like(Guid userId, Guid gameId);

        Task<bool> UnlikeDirectly(Guid userId, Guid gameId);

        Task Unlike(Guid userId, Guid gameId);

        Task<int> CountFollowers(Guid gameId);

        Task<int> CountLikes(Guid gameId);
    }
}