using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Team
{
    public class GetTeamQuery : GetBaseQuery<Models.Team>
    {
        public GetTeamQuery() : base()
        {
        }

        public GetTeamQuery(Expression<Func<Models.Team, bool>> where) : base(where)
        {
        }
    }

    public class GetTeamQueryHandler : SearchBaseQueryHandler<GetTeamQuery, Models.Team, ITeamRepository>
    {
        public GetTeamQueryHandler(ITeamRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.Team>> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}