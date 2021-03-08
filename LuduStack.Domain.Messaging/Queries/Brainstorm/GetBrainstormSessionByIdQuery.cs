using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.BrainstormSession
{
    public class GetBrainstormSessionByIdQuery : GetByIdBaseQuery<Models.BrainstormSession>
    {
        public GetBrainstormSessionByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetBrainstormSessionByIdQueryHandler : GetByIdBaseQueryHandler<GetBrainstormSessionByIdQuery, Models.BrainstormSession, IBrainstormRepository>
    {
        public GetBrainstormSessionByIdQueryHandler(IBrainstormRepository repository) : base(repository)
        {
        }
    }
}