using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.ForumGroup
{
    public class CountForumGroupQuery : CountBaseQuery<Models.ForumGroup>
    {
        public CountForumGroupQuery()
        {
        }

        public CountForumGroupQuery(Expression<Func<Models.ForumGroup, bool>> where) : base(where)
        {
        }
    }

    public class CountForumGroupQueryHandler : CountBaseQueryHandler<CountForumGroupQuery, Models.ForumGroup, IForumGroupRepository>
    {
        public CountForumGroupQueryHandler(IForumGroupRepository repository) : base(repository)
        {
        }
    }
}