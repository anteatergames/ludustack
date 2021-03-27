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
        public GetUserContentQuery() : base()
        {
        }

        public GetUserContentQuery(Expression<Func<Models.UserContent, bool>> where) : base(where)
        {
        }
    }

    public class GetUserContentQueryHandler : SearchBaseQueryHandler<GetUserContentQuery, Models.UserContent, IUserContentRepository>
    {
        public GetUserContentQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.UserContent>> Handle(GetUserContentQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}