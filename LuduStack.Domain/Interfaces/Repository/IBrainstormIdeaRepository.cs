using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IBrainstormIdeaRepository : IRepository<BrainstormIdea>
    {
        Task<BrainstormIdea> GetIdea(Guid ideaId);

        Task<bool> UpdateIdea(BrainstormIdea idea);

        Task<IEnumerable<BrainstormIdea>> GetIdeasBySession(Guid sessionId);

        Task<bool> AddVoteDirectly(BrainstormVote model);

        Task AddVote(BrainstormVote model);

        Task<bool> UpdateVoteDirectly(BrainstormVote model);

        Task UpdateVote(BrainstormVote model);

        Task AddComment(BrainstormComment model);

        Task<bool> AddCommentDirectly(BrainstormComment model);
    }
}