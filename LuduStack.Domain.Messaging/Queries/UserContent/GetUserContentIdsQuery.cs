using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetUserContentIdsQuery : GetIdsBaseQuery<Models.UserContent>
    {
        public GetUserContentIdsQuery()
        {
        }

        public GetUserContentIdsQuery(Expression<Func<Models.UserContent, bool>> where) : base(where)
        {
        }
    }

    public class GetUserContentIdsQueryHandler : GetIdsBaseQueryHandler<GetUserContentIdsQuery, Models.UserContent, IUserContentRepository>
    {
        public GetUserContentIdsQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetUserContentIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}