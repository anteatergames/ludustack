using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.ForumPost
{
    public class GetForumPostByIdQuery : GetByIdBaseQuery<Models.ForumPost>
    {
        public GetForumPostByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetForumPostByIdQueryHandler : GetByIdBaseQueryHandler<GetForumPostByIdQuery, Models.ForumPost, IForumPostRepository>
    {
        public GetForumPostByIdQueryHandler(IForumPostRepository repository) : base(repository)
        {
        }
    }
}