using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class GetGameQuery : GetBaseQuery<Models.Game>
    {
        public GetGameQuery() : base()
        {
        }

        public GetGameQuery(Expression<Func<Models.Game, bool>> where) : base(where)
        {
        }
    }

    public class GetGameQueryHandler : SearchBaseQueryHandler<GetGameQuery, Models.Game, IGameRepository>
    {
        public GetGameQueryHandler(IGameRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.Game>> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.Game> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}