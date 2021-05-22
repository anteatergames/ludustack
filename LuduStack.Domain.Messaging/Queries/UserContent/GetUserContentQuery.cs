using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetUserContentQuery : GetBaseQuery<Models.UserContent>
    {
        public GetUserContentQuery()
        {
        }

        public GetUserContentQuery(Expression<Func<Models.UserContent, bool>> where) : base(where)
        {
        }
    }

    public class GetUserContentQueryHandler : GetBaseQueryHandler<GetUserContentQuery, Models.UserContent, IUserContentRepository>
    {
        public GetUserContentQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.UserContent>> Handle(GetUserContentQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.UserContent> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}