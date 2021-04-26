using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Extensions;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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

        public override Task Add(UserContent obj)
        {
            if (obj.Language == 0)
            {
                obj.Language = SupportedLanguage.English;
            }

            return base.Add(obj);
        }

        public Task<int> CountComments(Expression<Func<UserContentComment, bool>> where)
        {
            int count = DbSet.AsQueryable().SelectMany(x => x.Comments).Where(where).Count();

            return Task.FromResult(count);
        }

        public Task<int> CountLikes(Expression<Func<UserContentLike, bool>> where)
        {
            int count = DbSet.AsQueryable().SelectMany(x => x.Likes).Where(where).Count();

            return Task.FromResult(count);
        }

        public async Task<List<UserContentComment>> GetComments(Expression<Func<UserContentComment, bool>> where)
        {
            List<UserContentComment> list = await DbSet.AsQueryable().SelectMany(x => x.Comments).Where(where).ToMongoListAsync();

            return list;
        }

        public Task<IQueryable<UserContentLike>> GetLikes(Expression<Func<UserContentLike, bool>> where)
        {
            IQueryable<UserContentLike> list = DbSet.AsQueryable().SelectMany(x => x.Likes).Where(where).AsQueryable();

            return Task.FromResult(list);
        }

        public async Task<bool> AddLike(UserContentLike model)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == model.ContentId);
            UpdateDefinition<UserContent> add = Builders<UserContent>.Update.AddToSet(c => c.Likes, model);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));

            return true;
        }

        public async Task<bool> RemoveLike(Guid userId, Guid userContentId)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == userContentId);
            UpdateDefinition<UserContent> remove = Builders<UserContent>.Update.PullFilter(c => c.Likes, m => m.UserId == userId);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, remove));

            return true;
        }

        public async Task<bool> AddComment(UserContentComment model)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == model.UserContentId);
            UpdateDefinition<UserContent> add = Builders<UserContent>.Update.AddToSet(c => c.Comments, model);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));

            return true;
        }

        public IQueryable<UserContentRating> GetRatings(Guid id)
        {
            IQueryable<UserContentRating> participants = DbSet.AsQueryable().Where(x => x.Id == id).SelectMany(x => x.Ratings);

            return participants;
        }

        public IQueryable<UserContentRating> GetRatings(Expression<Func<UserContent, bool>> where)
        {
            IQueryable<UserContentRating> participants = DbSet.AsQueryable().Where(where).SelectMany(x => x.Ratings);

            return participants;
        }

        public async Task<bool> UpdateRating(Guid id, UserContentRating rating)
        {
            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.And(
                Builders<UserContent>.Filter.Eq(x => x.Id, id),
                Builders<UserContent>.Filter.ElemMatch(x => x.Ratings, x => x.UserId == rating.UserId));

            UpdateDefinition<UserContent> update = Builders<UserContent>.Update
                .Set(c => c.Ratings[-1].Score, rating.Score);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));

            return true;
        }

        public async Task<bool> AddRating(Guid id, UserContentRating rating)
        {
            rating.Id = Guid.NewGuid();

            FilterDefinition<UserContent> filter = Builders<UserContent>.Filter.Where(x => x.Id == id);
            UpdateDefinition<UserContent> add = Builders<UserContent>.Update.AddToSet(c => c.Ratings, rating);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));

            return true;
        }
    }
}