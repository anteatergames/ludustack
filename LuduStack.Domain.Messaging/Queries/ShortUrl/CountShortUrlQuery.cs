using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;

namespace LuduStack.Domain.Messaging.Queries.ShortUrl
{
    public class CountShortUrlQuery : CountBaseQuery<Models.ShortUrl>
    {
        public CountShortUrlQuery()
        {
        }
    }

    public class CountShortUrlQueryHandler : CountBaseQueryHandler<CountShortUrlQuery, Models.ShortUrl, IShortUrlRepository>
    {
        public CountShortUrlQueryHandler(IShortUrlRepository repository) : base(repository)
        {
        }
    }
}