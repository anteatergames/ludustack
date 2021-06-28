using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class GetGameJamQuery : GetBaseQuery<Models.GameJam>
    {
        public GetGameJamQuery()
        {
        }

        public GetGameJamQuery(Expression<Func<Models.GameJam, bool>> where) : base(where)
        {
        }
    }

    public class GetGameJamQueryHandler : GetBaseQueryHandler<GetGameJamQuery, Models.GameJam, IGameJamRepository>
    {
        public GetGameJamQueryHandler(IGameJamRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.GameJam>> Handle(GetGameJamQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.GameJam> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}