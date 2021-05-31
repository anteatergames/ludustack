using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.ForumCategory
{
    public class CountForumCategoryQuery : CountBaseQuery<Models.ForumCategory>
    {
        public CountForumCategoryQuery()
        {
        }

        public CountForumCategoryQuery(Expression<Func<Models.ForumCategory, bool>> where) : base(where)
        {
        }
    }

    public class CountForumCategoryQueryHandler : CountBaseQueryHandler<CountForumCategoryQuery, Models.ForumCategory, IForumCategoryRepository>
    {
        public CountForumCategoryQueryHandler(IForumCategoryRepository repository) : base(repository)
        {
        }
    }
}