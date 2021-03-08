using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.FeaturedContent
{
    public class CountFeaturedContentQuery : CountBaseQuery<Models.FeaturedContent>
    {
        public CountFeaturedContentQuery() : base()
        {
        }
    }
    public class CountFeaturedContentQueryHandler : CountBaseQueryHandler<CountFeaturedContentQuery, Models.FeaturedContent, IFeaturedContentRepository>
    {
        public CountFeaturedContentQueryHandler(IFeaturedContentRepository repository) : base(repository)
        {
        }
    }
}
