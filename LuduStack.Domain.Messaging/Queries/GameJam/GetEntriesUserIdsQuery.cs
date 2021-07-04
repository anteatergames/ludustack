using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserProfile
{
    public class GetEntriesUserIdsQuery : GetUserIdsBaseQuery<Models.GameJamEntry>
    {
        public GetEntriesUserIdsQuery()
        {
        }

        public GetEntriesUserIdsQuery(Expression<Func<Models.GameJamEntry, bool>> where) : base(where)
        {
        }
    }

    public class GetEntriesUserIdsQueryHandler : GetUserIdsBaseQueryHandler<GetEntriesUserIdsQuery, Models.GameJamEntry, IGameJamEntryRepository>
    {
        public GetEntriesUserIdsQueryHandler(IGameJamEntryRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetEntriesUserIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}