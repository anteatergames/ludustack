using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.ShortUrl
{
    public class GetShortUrlQuery : GetBaseQuery<Models.ShortUrl>
    {
        public GetShortUrlQuery()
        {
        }

        public GetShortUrlQuery(Expression<Func<Models.ShortUrl, bool>> where) : base(where)
        {
        }
    }

    public class GetShortUrlQueryHandler : GetBaseQueryHandler<GetShortUrlQuery, Models.ShortUrl, IShortUrlRepository>
    {
        public GetShortUrlQueryHandler(IShortUrlRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.ShortUrl>> Handle(GetShortUrlQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.ShortUrl> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}