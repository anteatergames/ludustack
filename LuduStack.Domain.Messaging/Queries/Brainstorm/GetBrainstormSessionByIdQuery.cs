using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.Brainstorm
{
    public class GetBrainstormSessionByIdQuery : GetByIdBaseQuery<Models.BrainstormSession>
    {
        public GetBrainstormSessionByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetBrainstormSessionByIdQueryHandler : GetByIdBaseQueryHandler<GetBrainstormSessionByIdQuery, Models.BrainstormSession, IBrainstormSessionRepository>
    {
        public GetBrainstormSessionByIdQueryHandler(IBrainstormSessionRepository repository) : base(repository)
        {
        }
    }
}