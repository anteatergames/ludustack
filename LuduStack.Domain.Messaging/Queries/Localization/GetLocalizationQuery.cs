using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Localization
{
    public class GetLocalizationQuery : GetBaseQuery<Models.Localization>
    {
        public GetLocalizationQuery() : base()
        {
        }
    }
    public class GetLocalizationQueryHandler : SearchBaseQueryHandler<GetLocalizationQuery, Models.Localization, ILocalizationRepository>
    {
        public GetLocalizationQueryHandler(ILocalizationRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.Localization>> Handle(GetLocalizationQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}
