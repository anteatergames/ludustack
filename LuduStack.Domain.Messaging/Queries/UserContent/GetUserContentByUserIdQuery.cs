using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetUserContentByUserIdQuery : GetByUserIdBaseQuery<Models.UserContent>
    {
        public GetUserContentByUserIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetUserContentsByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetUserContentByUserIdQuery, Models.UserContent, IUserContentRepository>
    {
        public GetUserContentsByUserIdQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }
    }
}