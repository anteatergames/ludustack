using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.BrainstormSession
{
    public class GetBrainstormSessionIdsQuery : GetIdsBaseQuery<Models.BrainstormSession>
    {
        public GetBrainstormSessionIdsQuery() : base()
        {
        }

        public GetBrainstormSessionIdsQuery(Expression<Func<Models.BrainstormSession, bool>> where) : base(where)
        {
        }
    }

    public class GetBrainstormSessionIdsQueryHandler : GetIdsBaseQueryHandler<GetBrainstormSessionIdsQuery, Models.BrainstormSession, IBrainstormRepository>
    {
        public GetBrainstormSessionIdsQueryHandler(IBrainstormRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetBrainstormSessionIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}