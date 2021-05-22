using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.UserContent
{
    public class CountUserContentQuery : CountBaseQuery<Models.UserContent>
    {
        public CountUserContentQuery()
        {
        }

        public CountUserContentQuery(Expression<Func<Models.UserContent, bool>> where) : base(where)
        {
        }
    }

    public class CountUserContentQueryHandler : CountBaseQueryHandler<CountUserContentQuery, Models.UserContent, IUserContentRepository>
    {
        public CountUserContentQueryHandler(IUserContentRepository repository) : base(repository)
        {
        }
    }
}