using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.FeaturedContent
{
    public class GetFeaturedContentByIdQuery : GetByIdBaseQuery<Models.FeaturedContent>
    {
        public GetFeaturedContentByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetFeaturedContentByIdQueryHandler : GetByIdBaseQueryHandler<GetFeaturedContentByIdQuery, Models.FeaturedContent, IFeaturedContentRepository>
    {
        public GetFeaturedContentByIdQueryHandler(IFeaturedContentRepository repository) : base(repository)
        {
        }
    }
}