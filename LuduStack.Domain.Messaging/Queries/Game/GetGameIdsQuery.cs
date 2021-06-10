using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class GetGameIdsQuery : GetIdsBaseQuery<Models.Game>
    {
        public GetGameIdsQuery()
        {
        }

        public GetGameIdsQuery(Expression<Func<Models.Game, bool>> where) : base(where)
        {
        }
    }

    public class GetGameIdsQueryHandler : GetIdsBaseQueryHandler<GetGameIdsQuery, Models.Game, IGameRepository>
    {
        public GetGameIdsQueryHandler(IGameRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetGameIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}