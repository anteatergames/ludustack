using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IPollRepository : IRepository<Poll>
    {
        Poll GetPollByOptionId(Guid optionId);

        Task<bool> AddVote(Guid pollId, PollVote vote);

        Task<bool> RemoveVote(Guid userId, Guid pollId);

        Task<bool> UpdateVote(PollVote vote);

        int CountVotes(Func<PollVote, bool> where);

        IQueryable<PollVote> GetVotes(Guid pollId);

        void RemoveByContentId(Guid contentId);

        PollVote GetVote(Guid userId, Guid pollId);

        Task<IEnumerable<Poll>> GetPollsByUserContentIds(List<Guid> userContentIds);
    }
}