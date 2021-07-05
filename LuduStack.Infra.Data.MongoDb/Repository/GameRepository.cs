using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GameRepository : BaseRepository<Game>, IGameRepository
    {
        public GameRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<bool> FollowDirectly(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> followerRemove = Builders<Game>.Update.AddToSet(c => c.Followers, new GameFollow { UserId = userId, GameId = gameId });

            UpdateResult result = await DbSet.UpdateOneAsync(gameFilter, followerRemove);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task Follow(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> follow = Builders<Game>.Update.AddToSet(c => c.Followers, new GameFollow { UserId = userId, GameId = gameId });

            await Context.AddCommand(() => DbSet.UpdateOneAsync(gameFilter, follow));
        }

        public async Task<bool> UnfollowDirectly(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> followerRemove = Builders<Game>.Update.PullFilter(c => c.Followers, m => m.UserId == userId);

            UpdateResult result = await DbSet.UpdateOneAsync(gameFilter, followerRemove);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task Unfollow(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> unfollow = Builders<Game>.Update.PullFilter(c => c.Followers, m => m.UserId == userId);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(gameFilter, unfollow));
        }

        public async Task<bool> LikeDirectly(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> like = Builders<Game>.Update.AddToSet(c => c.Likes, new GameLike { UserId = userId, GameId = gameId });

            UpdateResult result = await DbSet.UpdateOneAsync(gameFilter, like);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task Like(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> like = Builders<Game>.Update.AddToSet(c => c.Likes, new GameLike { UserId = userId, GameId = gameId });

            await Context.AddCommand(() => DbSet.UpdateOneAsync(gameFilter, like));
        }

        public async Task<bool> UnlikeDirectly(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> unlike = Builders<Game>.Update.PullFilter(c => c.Likes, m => m.UserId == userId);

            UpdateResult result = await DbSet.UpdateOneAsync(gameFilter, unlike);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task Unlike(Guid userId, Guid gameId)
        {
            FilterDefinition<Game> gameFilter = Builders<Game>.Filter.Where(x => x.Id == gameId);
            UpdateDefinition<Game> unlike = Builders<Game>.Update.PullFilter(c => c.Likes, m => m.UserId == userId);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(gameFilter, unlike));
        }

        public Task<int> CountFollowers(Guid gameId)
        {
            int result = DbSet.AsQueryable().Where(x => x.Id == gameId).SelectMany(x => x.Followers).Count();

            return Task.FromResult(result);
        }

        public Task<int> CountLikes(Guid gameId)
        {
            int result = DbSet.AsQueryable().Where(x => x.Id == gameId).SelectMany(x => x.Likes).Count();

            return Task.FromResult(result);
        }

        public async Task<IEnumerable<Game>> GetByIds(List<Guid> ids)
        {
            ProjectionDefinition<Game, Game> projection1 =
                Builders<Game>.Projection.Expression(x => new Game
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CreateDate = x.CreateDate,
                    Title = x.Title,
                    ThumbnailUrl = x.ThumbnailUrl
                });

            FindOptions<Game, Game> findOptions = new FindOptions<Game, Game>
            {
                Projection = projection1
            };

            FilterDefinition<Game> filter = new ExpressionFilterDefinition<Game>(x => ids.Contains(x.Id));

            List<Game> games = await (await DbSet.FindAsync(filter, findOptions)).ToListAsync();

            return games;
        }
    }
}