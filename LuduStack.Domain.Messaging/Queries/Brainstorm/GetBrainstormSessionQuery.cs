using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Brainstorm
{
    public class GetBrainstormSessionQuery : GetBaseQuery<Models.BrainstormSession>
    {
        public GetBrainstormSessionQuery()
        {
        }

        public GetBrainstormSessionQuery(Expression<Func<Models.BrainstormSession, bool>> where) : base(where)
        {
        }
    }

    public class GetBrainstormSessionQueryHandler : GetBaseQueryHandler<GetBrainstormSessionQuery, Models.BrainstormSession, IBrainstormSessionRepository>
    {
        public GetBrainstormSessionQueryHandler(IBrainstormSessionRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.BrainstormSession>> Handle(GetBrainstormSessionQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.BrainstormSession> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}