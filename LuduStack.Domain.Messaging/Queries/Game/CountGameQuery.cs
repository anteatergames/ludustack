using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.Game
{
    public class CountGameQuery : CountBaseQuery<Models.Game>
    {
        public CountGameQuery()
        {
        }

        public CountGameQuery(Expression<Func<Models.Game, bool>> where) : base(where)
        {
        }
    }

    public class CountGameQueryHandler : CountBaseQueryHandler<CountGameQuery, Models.Game, IGameRepository>
    {
        public CountGameQueryHandler(IGameRepository repository) : base(repository)
        {
        }
    }
}