using LuduStack.Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IGameJamEntryRepository : IRepository<GameJamEntry>
    {
        IQueryable<GameJamTeamMember> GetParticipants(Expression<Func<GameJamEntry, bool>> where);

        IQueryable<GameJamVote> GetVotes(Expression<Func<GameJamEntry, bool>> where);

        Task<bool> UpdateRating(Guid id, GameJamVote vote);

        Task<bool> AddRating(Guid id, GameJamVote vote);
    }
}