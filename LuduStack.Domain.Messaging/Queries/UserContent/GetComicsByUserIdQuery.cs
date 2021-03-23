using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using System;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class GetComicsByUserIdQuery : GetUserContentQuery
    {
        public GetComicsByUserIdQuery(Guid userId) : base(x => x.UserId == userId && x.UserContentType == UserContentType.ComicStrip)
        {
        }
    }

    public class GetComicsByUserIdQueryHandler : GetUserContentQueryHandler<GetComicsByUserIdQuery>
    {
        public GetComicsByUserIdQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }
    }
}