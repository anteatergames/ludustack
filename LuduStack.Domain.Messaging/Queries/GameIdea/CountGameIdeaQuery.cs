using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.GameIdea
{
    public class CountGameIdeaQuery : CountBaseQuery<Models.GameIdea>
    {
        public CountGameIdeaQuery()
        {
        }

        public CountGameIdeaQuery(Expression<Func<Models.GameIdea, bool>> where) : base(where)
        {
        }
    }

    public class CountGameIdeaQueryHandler : CountBaseQueryHandler<CountGameIdeaQuery, Models.GameIdea, IGameIdeaRepository>
    {
        public CountGameIdeaQueryHandler(IGameIdeaRepository repository) : base(repository)
        {
        }
    }
}