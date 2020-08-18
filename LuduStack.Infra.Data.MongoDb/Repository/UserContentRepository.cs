using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class UserContentRepository : BaseRepository<UserContent>, IUserContentRepository
    {
        public UserContentRepository(IMongoContext context) : base(context)
        {
        }

        public override void Add(UserContent obj)
        {
            if (obj.Language == 0)
            {
                obj.Language = SupportedLanguage.English;
            }

            base.Add(obj);
        }

        public Task<int> CountComments(Expression<Func<UserContentComment, bool>> where)
        {
            int count = DbSet.AsQueryable().SelectMany(x => x.Comments).Where(where).Count();

            return Task.FromResult(count);
        }

        public Task<IQueryable<UserContentComment>> GetComments(Expression<Func<UserContentComment, bool>> where)
        {
            IQueryable<UserContentComment> list = DbSet.AsQueryable().SelectMany(x => x.Comments).Where(where).AsQueryable();

            return Task.FromResult(list);
        }

        public Task<IQueryable<UserContentLike>> GetLikes(Func<UserContentLike, bool> where)
        {
            IQueryable<UserContentLike> list = DbSet.AsQueryable().SelectMany(x => x.Likes).Where(where).AsQueryable();

            return Task.FromResult(list);
        }

        public async Task<bool> AddLike(UserContentLike model)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == model.ContentId);
            UpdateDefinition<UserContent> add = Builders<UserContent>.Update.AddToSet(c => c.Likes, model);

            UpdateResult result = await DbSet.UpdateOneAsync(filter, add);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task<bool> RemoveLike(Guid userId, Guid userContentId)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == userContentId);
            UpdateDefinition<UserContent> remove = Builders<UserContent>.Update.PullFilter(c => c.Likes, m => m.UserId == userId);

            UpdateResult result = await DbSet.UpdateOneAsync(filter, remove);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task<bool> AddComment(UserContentComment model)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == model.UserContentId);
            UpdateDefinition<UserContent> add = Builders<UserContent>.Update.AddToSet(c => c.Comments, model);

            UpdateResult result = await DbSet.UpdateOneAsync(filter, add);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public IQueryable<UserContentRating> GetRatings(Guid id)
        {
            IQueryable<UserContentRating> participants = DbSet.AsQueryable().Where(x => x.Id == id).SelectMany(x => x.Ratings);

            return participants;
        }

        public void UpdateRating(Guid id, UserContentRating rating)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.And(
                Builders<UserContent>.Filter.Eq(x => x.Id, id),
                Builders<UserContent>.Filter.ElemMatch(x => x.Ratings, x => x.UserId == rating.UserId));

            UpdateDefinition<UserContent> update = Builders<UserContent>.Update
                .Set(c => c.Ratings[-1].Score, rating.Score);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));
        }

        public void AddRating(Guid id, UserContentRating rating)
        {
            rating.Id = Guid.NewGuid();

            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == id);
            UpdateDefinition<UserContent> add = Builders<UserContent>.Update.AddToSet(c => c.Ratings, rating);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));
        }
    }
}