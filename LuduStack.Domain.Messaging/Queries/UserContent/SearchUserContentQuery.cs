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

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class SearchUserContentQuery : GetBaseQuery<Models.UserContent>
    {
        public SearchUserContentQuery(Expression<Func<Models.UserContent, bool>> where) : base(where)
        {
        }
    }
    public class SearchUserContentQueryHandler : SearchBaseQueryHandler<SearchUserContentQuery, Models.UserContent, IUserContentRepository>
    {
        public SearchUserContentQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.UserContent>> Handle(SearchUserContentQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}
