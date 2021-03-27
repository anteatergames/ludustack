using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserPreferences
{
    public class GetUserPreferencesQuery : GetBaseQuery<Models.UserPreferences>
    {
        public GetUserPreferencesQuery() : base()
        {
        }

        public GetUserPreferencesQuery(Expression<Func<Models.UserPreferences, bool>> where) : base(where)
        {
        }
    }

    public class GetUserPreferencesQueryHandler : SearchBaseQueryHandler<GetUserPreferencesQuery, Models.UserPreferences, IUserPreferencesRepository>
    {
        public GetUserPreferencesQueryHandler(IUserPreferencesRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.UserPreferences>> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}