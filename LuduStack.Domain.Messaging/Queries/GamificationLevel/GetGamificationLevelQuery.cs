using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.GamificationLevel
{
    public class GetGamificationLevelQuery : GetBaseQuery<Models.GamificationLevel>
    {
        public GetGamificationLevelQuery()
        {
        }

        public GetGamificationLevelQuery(Expression<Func<Models.GamificationLevel, bool>> where) : base(where)
        {
        }
    }

    public class GetGamificationLevelQueryHandler : GetBaseQueryHandler<GetGamificationLevelQuery, Models.GamificationLevel, IGamificationLevelRepository>
    {
        public GetGamificationLevelQueryHandler(IGamificationLevelRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.GamificationLevel>> Handle(GetGamificationLevelQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.GamificationLevel> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}