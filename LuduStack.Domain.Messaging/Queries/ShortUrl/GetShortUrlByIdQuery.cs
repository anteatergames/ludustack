using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.ShortUrl
{
    public class GetShortUrlByIdQuery : GetByIdBaseQuery<Models.ShortUrl>
    {
        public GetShortUrlByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetShortUrlByIdQueryHandler : GetByIdBaseQueryHandler<GetShortUrlByIdQuery, Models.ShortUrl, IShortUrlRepository>
    {
        public GetShortUrlByIdQueryHandler(IShortUrlRepository repository) : base(repository)
        {
        }
    }
}