using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.ForumPost
{
    public class CountForumPostQuery : CountBaseQuery<Models.ForumPost>
    {
        public CountForumPostQuery()
        {
        }

        public CountForumPostQuery(Expression<Func<Models.ForumPost, bool>> where) : base(where)
        {
        }
    }

    public class CountForumPostQueryHandler : CountBaseQueryHandler<CountForumPostQuery, Models.ForumPost, IForumPostRepository>
    {
        public CountForumPostQueryHandler(IForumPostRepository repository) : base(repository)
        {
        }
    }
}