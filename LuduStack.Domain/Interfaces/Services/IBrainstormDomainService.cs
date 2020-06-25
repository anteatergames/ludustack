using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Interfaces.Services
{
    public interface IBrainstormDomainService : IDomainService<BrainstormSession>
    {
        BrainstormIdea GetIdea(Guid ideaId);

        void AddIdea(BrainstormIdea model);

        void UpdateIdea(BrainstormIdea idea);

        IEnumerable<BrainstormIdea> GetIdeasBySession(Guid sessionId);

        void AddVote(BrainstormVote model);

        void UpdateVote(BrainstormVote model);

        void AddComment(BrainstormComment model);

        BrainstormSession Get(BrainstormSessionType type);

        Guid GetUserId(Guid sessionId);
    }
}