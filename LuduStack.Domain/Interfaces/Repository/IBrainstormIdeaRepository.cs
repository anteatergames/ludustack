using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Domain.Interfaces.Repository
{
    public interface IBrainstormIdeaRepository : IRepository<BrainstormIdea>
    {
        Task<BrainstormIdea> GetIdea(Guid ideaId);

        Task<bool> UpdateIdeaDirectly(BrainstormIdea idea);

        Task UpdateIdea(BrainstormIdea idea);

        Task<IEnumerable<BrainstormIdea>> GetIdeasBySession(Guid sessionId);

        Task<bool> AddVoteDirectly(Guid ideaId, UserVoteVo model);

        Task AddVote(Guid ideaId, UserVoteVo model);

        Task<bool> UpdateVoteDirectly(Guid ideaId, UserVoteVo model);

        Task UpdateVote(Guid ideaId, UserVoteVo model);

        Task AddComment(BrainstormComment model);

        Task<bool> AddCommentDirectly(BrainstormComment model);
    }
}