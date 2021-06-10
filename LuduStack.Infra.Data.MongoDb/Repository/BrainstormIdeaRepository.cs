using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class BrainstormIdeaRepository : BaseRepository<BrainstormIdea>, IBrainstormIdeaRepository
    {
        public BrainstormIdeaRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<BrainstormIdea> GetIdea(Guid ideaId)
        {
            BrainstormIdea idea = await GetCollection<BrainstormIdea>().FindSync(x => x.Id == ideaId).FirstOrDefaultAsync();

            return idea;
        }

        public async Task<IEnumerable<BrainstormIdea>> GetIdeasBySession(Guid sessionId)
        {
            List<BrainstormIdea> ideas = await GetCollection<BrainstormIdea>().Find(x => x.SessionId == sessionId).ToListAsync();

            return ideas;
        }

        public async Task<bool> UpdateIdeaDirectly(BrainstormIdea idea)
        {
            ReplaceOneResult result = await GetCollection<BrainstormIdea>().ReplaceOneAsync(x => x.Id == idea.Id, idea);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task UpdateIdea(BrainstormIdea idea)
        {
            FilterDefinition<BrainstormIdea> filter = Builders<BrainstormIdea>.Filter.Where(x => x.Id == idea.Id);

            await Context.AddCommand(() => DbSet.ReplaceOneAsync(filter, idea));
        }

        public async Task<bool> AddVoteDirectly(Guid ideaId, UserVoteVo model)
        {
            FilterDefinition<BrainstormIdea> filter = Builders<BrainstormIdea>.Filter.Where(x => x.Id == ideaId);
            UpdateDefinition<BrainstormIdea> add = Builders<BrainstormIdea>.Update.AddToSet(c => c.Votes, model);

            UpdateResult result = await GetCollection<BrainstormIdea>().UpdateOneAsync(filter, add);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task AddVote(Guid ideaId, UserVoteVo model)
        {
            FilterDefinition<BrainstormIdea> filter = Builders<BrainstormIdea>.Filter.Where(x => x.Id == ideaId);
            UpdateDefinition<BrainstormIdea> add = Builders<BrainstormIdea>.Update.AddToSet(c => c.Votes, model);

            await DbSet.UpdateOneAsync(filter, add);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));
        }

        public async Task<bool> UpdateVoteDirectly(Guid ideaId, UserVoteVo model)
        {
            FilterDefinition<BrainstormIdea> filter = Builders<BrainstormIdea>.Filter.And(
                Builders<BrainstormIdea>.Filter.Eq(x => x.Id, ideaId),
                Builders<BrainstormIdea>.Filter.ElemMatch(x => x.Votes, x => x.UserId == model.UserId));

            UpdateDefinition<BrainstormIdea> update = Builders<BrainstormIdea>.Update.Set(c => c.Votes[-1].VoteValue, model.VoteValue);

            UpdateResult result = await GetCollection<BrainstormIdea>().UpdateOneAsync(filter, update);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task UpdateVote(Guid ideaId, UserVoteVo model)
        {
            FilterDefinition<BrainstormIdea> filter = Builders<BrainstormIdea>.Filter.And(
                Builders<BrainstormIdea>.Filter.Eq(x => x.Id, ideaId),
                Builders<BrainstormIdea>.Filter.ElemMatch(x => x.Votes, x => x.UserId == model.UserId));

            UpdateDefinition<BrainstormIdea> update = Builders<BrainstormIdea>.Update.Set(c => c.Votes[-1].VoteValue, model.VoteValue);

            await DbSet.UpdateOneAsync(filter, update);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));
        }

        public async Task AddComment(BrainstormComment model)
        {
            FilterDefinition<BrainstormIdea> filter = Builders<BrainstormIdea>.Filter.Where(x => x.Id == model.IdeaId);
            UpdateDefinition<BrainstormIdea> add = Builders<BrainstormIdea>.Update.AddToSet(c => c.Comments, model);

            await Context.AddCommand(() => GetCollection<BrainstormIdea>().UpdateOneAsync(filter, add));
        }

        public async Task<bool> AddCommentDirectly(BrainstormComment model)
        {
            FilterDefinition<BrainstormIdea> filter = Builders<BrainstormIdea>.Filter.Where(x => x.Id == model.IdeaId);
            UpdateDefinition<BrainstormIdea> add = Builders<BrainstormIdea>.Update.AddToSet(c => c.Comments, model);

            UpdateResult result = await GetCollection<BrainstormIdea>().UpdateOneAsync(filter, add);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public override async Task Add(BrainstormIdea obj)
        {
            if (obj.Status == 0)
            {
                obj.Status = BrainstormIdeaStatus.Proposed;
            }

            await base.Add(obj);
        }
    }
}