using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Localization
{
    public class GetLocalizationIdsQuery : GetIdsBaseQuery<Models.Localization>
    {
        public GetLocalizationIdsQuery()
        {
        }

        public GetLocalizationIdsQuery(Expression<Func<Models.Localization, bool>> where) : base(where)
        {
        }
    }

    public class GetLocalizationIdsQueryHandler : GetIdsBaseQueryHandler<GetLocalizationIdsQuery, Models.Localization, ILocalizationRepository>
    {
        public GetLocalizationIdsQueryHandler(ILocalizationRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetLocalizationIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}