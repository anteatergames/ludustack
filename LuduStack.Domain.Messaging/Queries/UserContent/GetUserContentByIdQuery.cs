using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetUserContentByIdQuery : GetByIdBaseQuery<Models.UserContent>
    {
        public GetUserContentByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetUserContentByIdQueryHandler : GetByIdBaseQueryHandler<GetUserContentByIdQuery, Models.UserContent, IUserContentRepository>
    {
        public GetUserContentByIdQueryHandler(IUserContentRepository userContentRepository) : base(userContentRepository)
        {
        }
    }
}