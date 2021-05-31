using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ForumCategory
{
    public class GetForumCategoryQuery : GetBaseQuery<Models.ForumCategory>
    {
        public GetForumCategoryQuery()
        {
        }

        public GetForumCategoryQuery(Expression<Func<Models.ForumCategory, bool>> where) : base(where)
        {
        }
    }

    public class GetForumCategoryQueryHandler : GetBaseQueryHandler<GetForumCategoryQuery, Models.ForumCategory, IForumCategoryRepository>
    {
        public GetForumCategoryQueryHandler(IForumCategoryRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.ForumCategory>> Handle(GetForumCategoryQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.ForumCategory> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}