using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Brainstorm
{
    public class GetBrainstormIdeaByIdQuery : GetByIdBaseQuery<Models.BrainstormIdea>
    {
        public GetBrainstormIdeaByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetBrainstormIdeaByIdQueryHandler : GetByIdBaseQueryHandler<GetBrainstormIdeaByIdQuery, Models.BrainstormIdea, IBrainstormIdeaRepository>
    {
        public GetBrainstormIdeaByIdQueryHandler(IBrainstormIdeaRepository repository) : base(repository)
        {
        }
    }
}