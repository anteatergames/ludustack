using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class ForumPostRepository : BaseRepository<ForumPost>, IForumPostRepository
    {
        public ForumPostRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<ForumCategoryConterDataVo>> GetForumCategoryCounterInformation(List<SupportedLanguage> languages)
        {
            ProjectionDefinition<ForumPost, ForumCategoryConterDataVo> projection1 =
                Builders<ForumPost>.Projection.Expression(x => new ForumCategoryConterDataVo
                {
                    Id = x.Id,
                    OriginalPostId = x.OriginalPostId,
                    UserId = x.UserId,
                    CreateDate = x.CreateDate,
                    ForumCategoryId = x.ForumCategoryId,
                    IsOriginalPost = x.IsOriginalPost,
                    Title = x.Title
                });

            FindOptions<ForumPost, ForumCategoryConterDataVo> findOptions = new FindOptions<ForumPost, ForumCategoryConterDataVo>
            {
                Projection = projection1
            };

            FilterDefinition<ForumPost> filter;

            if (languages != null && languages.Any())
            {
                filter = new ExpressionFilterDefinition<ForumPost>(x => languages.Contains(x.Language));
            }
            else
            {
                filter = new ExpressionFilterDefinition<ForumPost>(x => true);
            }

            List<ForumCategoryConterDataVo> data = await (await DbSet.FindAsync(filter, findOptions)).ToListAsync();

            return data;
        }

        public async Task<List<ForumPostConterDataVo>> GetForumPostCounterInformation(Guid? forumCategoryId)
        {
            ProjectionDefinition<ForumPost, ForumPostConterDataVo> projection1 =
                Builders<ForumPost>.Projection.Expression(x => new ForumPostConterDataVo
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    CreateDate = x.CreateDate,
                    IsOriginalPost = x.IsOriginalPost,
                    OriginalPostId = x.OriginalPostId,
                    Title = x.Title,
                    Views = x.Views.Count
                });

            FindOptions<ForumPost, ForumPostConterDataVo> findOptions = new FindOptions<ForumPost, ForumPostConterDataVo>
            {
                Projection = projection1
            };

            FilterDefinition<ForumPost> filter;

            if (forumCategoryId.HasValue)
            {
                filter = new ExpressionFilterDefinition<ForumPost>(x => x.ForumCategoryId == forumCategoryId.Value);
            }
            else
            {
                filter = new ExpressionFilterDefinition<ForumPost>(x => true);
            }

            List<ForumPostConterDataVo> data = await (await DbSet.FindAsync(filter, findOptions)).ToListAsync();

            return data;
        }

        public bool CanRegisterViewForUser(Guid forumPostId, Guid userId)
        {
            UserViewVo existingView = DbSet.AsQueryable().Where(x => x.Id == forumPostId).SelectMany(x => x.Views).FirstOrDefault(x => x.UserId == userId);

            return existingView == null ? true : false;
        }

        public async Task RegisterView(Guid forumPostId, Guid? userId)
        {
            UserViewVo model = new UserViewVo
            {
                CreateDate = DateTime.Now,
                UserId = userId
            };

            FilterDefinition<ForumPost> filter = Builders<ForumPost>.Filter.Where(x => x.Id == forumPostId);
            UpdateDefinition<ForumPost> add = Builders<ForumPost>.Update.AddToSet(c => c.Views, model);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));
        }

        public async Task AddVote(Guid forumPostId, UserVoteVo model)
        {
            FilterDefinition<ForumPost> filter = Builders<ForumPost>.Filter.Where(x => x.Id == forumPostId);
            UpdateDefinition<ForumPost> add = Builders<ForumPost>.Update.AddToSet(c => c.Votes, model);

            await DbSet.UpdateOneAsync(filter, add);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));
        }

        public async Task UpdateVote(Guid forumPostId, UserVoteVo model)
        {
            FilterDefinition<ForumPost> filter = Builders<ForumPost>.Filter.And(
                Builders<ForumPost>.Filter.Eq(x => x.Id, forumPostId),
                Builders<ForumPost>.Filter.ElemMatch(x => x.Votes, x => x.UserId == model.UserId));

            UpdateDefinition<ForumPost> update = Builders<ForumPost>.Update.Set(c => c.Votes[-1].VoteValue, model.VoteValue);

            await DbSet.UpdateOneAsync(filter, update);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));
        }

        public Task<int> GetScore(Guid forumPostId)
        {
            IQueryable<ForumPost> forumPost = DbSet.AsQueryable().Where(x => x.Id == forumPostId);

            List<Domain.Core.Enums.VoteValue> votes = forumPost.SelectMany(x => x.Votes).Select(x => x.VoteValue).ToList();

            int score = votes.Sum(x => (int)x);

            return Task.FromResult(score);
        }
    }
}