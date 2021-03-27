using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.BrainstormSession
{
    public class GetBrainstormSessionQuery : GetBaseQuery<Models.BrainstormSession>
    {
        public GetBrainstormSessionQuery() : base()
        {
        }

        public GetBrainstormSessionQuery(Expression<Func<Models.BrainstormSession, bool>> where) : base(where)
        {
        }
    }

    public class GetBrainstormSessionQueryHandler : SearchBaseQueryHandler<GetBrainstormSessionQuery, Models.BrainstormSession, IBrainstormRepository>
    {
        public GetBrainstormSessionQueryHandler(IBrainstormRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.BrainstormSession>> Handle(GetBrainstormSessionQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}