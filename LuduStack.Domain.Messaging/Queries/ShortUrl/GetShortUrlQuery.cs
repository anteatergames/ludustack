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
        public GetShortUrlQuery() : base()
        {
        }

        public GetShortUrlQuery(Expression<Func<Models.ShortUrl, bool>> where) : base(where)
        {
        }
    }

    public class GetShortUrlQueryHandler : SearchBaseQueryHandler<GetShortUrlQuery, Models.ShortUrl, IShortUrlRepository>
    {
        public GetShortUrlQueryHandler(IShortUrlRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.ShortUrl>> Handle(GetShortUrlQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}