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
    public class GameJamEntryRepository : BaseRepository<GameJamEntry>, IGameJamEntryRepository
    {
        public GameJamEntryRepository(IMongoContext context) : base(context)
        {
        }

        public IQueryable<GameJamTeamMember> GetParticipants(Expression<Func<GameJamEntry, bool>> where)
        {
            IQueryable<GameJamTeamMember> participants = DbSet.AsQueryable().Where(where).SelectMany(x => x.TeamMembers);

            return participants;
        }

        public IQueryable<GameJamVote> GetVotes(Expression<Func<GameJamEntry, bool>> where)
        {
            IQueryable<GameJamVote> votes = DbSet.AsQueryable().Where(where).SelectMany(x => x.Votes);

            return votes;
        }

        public async Task<bool> UpdateRating(Guid id, GameJamVote vote)
        {
            FilterDefinition<GameJamEntry> filter = Builders<GameJamEntry>.Filter.And(
                Builders<GameJamEntry>.Filter.Eq(x => x.Id, id),
                Builders<GameJamEntry>.Filter.ElemMatch(x => x.Votes, x => x.UserId == vote.UserId && x.CriteriaType == vote.CriteriaType));

            UpdateDefinition<GameJamEntry> update = Builders<GameJamEntry>.Update
                .Set(c => c.Votes[-1].Score, vote.Score);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));

            return true;
        }

        public async Task<bool> AddRating(Guid id, GameJamVote vote)
        {
            FilterDefinition<GameJamEntry> filter = Builders<GameJamEntry>.Filter.Where(x => x.Id == id);
            UpdateDefinition<GameJamEntry> add = Builders<GameJamEntry>.Update.AddToSet(c => c.Votes, vote);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));

            return true;
        }
    }
}