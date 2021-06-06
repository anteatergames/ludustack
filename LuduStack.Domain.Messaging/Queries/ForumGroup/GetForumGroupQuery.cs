using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumGroup
{
    public class GetForumGroupQuery : GetBaseQuery<Models.ForumGroup>
    {
        public GetForumGroupQuery()
        {
        }

        public GetForumGroupQuery(Expression<Func<Models.ForumGroup, bool>> where) : base(where)
        {
        }
    }

    public class GetForumGroupQueryHandler : GetBaseQueryHandler<GetForumGroupQuery, Models.ForumGroup, IForumGroupRepository>
    {
        public GetForumGroupQueryHandler(IForumGroupRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.ForumGroup>> Handle(GetForumGroupQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.ForumGroup> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}