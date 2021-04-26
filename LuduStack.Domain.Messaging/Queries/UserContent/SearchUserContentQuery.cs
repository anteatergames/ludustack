using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class SearchUserContentQuery : GetBaseQuery<Models.UserContent>
    {
        public SearchUserContentQuery(Expression<Func<Models.UserContent, bool>> where) : base(where)
        {
        }
    }

    public class SearchUserContentQueryHandler : GetBaseQueryHandler<SearchUserContentQuery, Models.UserContent, IUserContentRepository>
    {
        public SearchUserContentQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.UserContent>> Handle(SearchUserContentQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.UserContent> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}