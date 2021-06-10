using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumPost
{
    public class GetForumPostQuery : GetBaseQuery<Models.ForumPost>
    {
        public GetForumPostQuery()
        {
        }

        public GetForumPostQuery(Expression<Func<Models.ForumPost, bool>> where) : base(where)
        {
        }
    }

    public class GetForumPostQueryHandler : GetBaseQueryHandler<GetForumPostQuery, Models.ForumPost, IForumPostRepository>
    {
        public GetForumPostQueryHandler(IForumPostRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.ForumPost>> Handle(GetForumPostQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.ForumPost> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}