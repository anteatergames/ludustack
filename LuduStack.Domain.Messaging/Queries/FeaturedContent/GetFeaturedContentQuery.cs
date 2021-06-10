using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.FeaturedContent
{
    public class GetFeaturedContentQuery : GetBaseQuery<Models.FeaturedContent>
    {
        public GetFeaturedContentQuery()
        {
        }

        public GetFeaturedContentQuery(Expression<Func<Models.FeaturedContent, bool>> where) : base(where)
        {
        }
    }

    public class GetFeaturedContentQueryHandler : GetBaseQueryHandler<GetFeaturedContentQuery, Models.FeaturedContent, IFeaturedContentRepository>
    {
        public GetFeaturedContentQueryHandler(IFeaturedContentRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.FeaturedContent>> Handle(GetFeaturedContentQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.FeaturedContent> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}