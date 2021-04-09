using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;
using System.Linq;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IGameDomainService
    {
        IQueryable<Game> Get(GameGenre genre, Guid userId, Guid? teamId);

        bool Follow(Guid userId, Guid gameId);

        bool Unfollow(Guid userId, Guid gameId);

        bool Like(Guid userId, Guid gameId);

        bool Unlike(Guid userId, Guid gameId);

        int CountFollowers(Guid gameId);

        int CountLikes(Guid gameId);
    }
}