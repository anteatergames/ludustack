using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Services
{
    public class GameDomainService : BaseDomainMongoService<Game, IGameRepository>, IGameDomainService
    {
        public GameDomainService(IGameRepository repository) : base(repository)
        {
        }

        public IQueryable<Game> Get(GameGenre genre, Guid userId, Guid? teamId)
        {
            IQueryable<Game> allModels = repository.Get();

            if (genre != 0)
            {
                allModels = allModels.Where(x => x.Genre == genre);
            }

            if (userId != Guid.Empty)
            {
                allModels = allModels.Where(x => x.UserId == userId);
            }

            if (teamId.HasValue)
            {
                allModels = allModels.Where(x => x.TeamId == teamId);
            }

            return allModels;
        }

        public int CountFollowers(Guid gameId)
        {
            Task<int> task = Task.Run(async () => await repository.CountFollowers(gameId));

            return task.Result;
        }

        public int CountLikes(Guid gameId)
        {
            Task<int> task = Task.Run(async () => await repository.CountLikes(gameId));

            return task.Result;
        }

        public bool Follow(Guid userId, Guid gameId)
        {
            Task<bool> task = Task.Run(async () => await repository.Follow(userId, gameId));

            return task.Result;
        }

        public bool Like(Guid userId, Guid gameId)
        {
            Task<bool> task = Task.Run(async () => await repository.Like(userId, gameId));

            return task.Result;
        }

        public bool Unfollow(Guid userId, Guid gameId)
        {
            Task<bool> task = Task.Run(async () => await repository.Unfollow(userId, gameId));

            return task.Result;
        }

        public bool Unlike(Guid userId, Guid gameId)
        {
            Task<bool> task = Task.Run(async () => await repository.Unlike(userId, gameId));

            return task.Result;
        }
    }
}