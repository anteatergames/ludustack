using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Localization
{
    public class GetLocalizationQuery : GetBaseQuery<Models.Localization>
    {
        public GetLocalizationQuery()
        {
        }

        public GetLocalizationQuery(Expression<Func<Models.Localization, bool>> where) : base(where)
        {
        }
    }

    public class GetLocalizationQueryHandler : GetBaseQueryHandler<GetLocalizationQuery, Models.Localization, ILocalizationRepository>
    {
        public GetLocalizationQueryHandler(ILocalizationRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.Localization>> Handle(GetLocalizationQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.Localization> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}