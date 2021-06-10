using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.GamificationLevel
{
    public class CountGamificationLevelQuery : CountBaseQuery<Models.GamificationLevel>
    {
        public CountGamificationLevelQuery()
        {
        }

        public CountGamificationLevelQuery(Expression<Func<Models.GamificationLevel, bool>> where) : base(where)
        {
        }
    }

    public class CountGamificationLevelQueryHandler : CountBaseQueryHandler<CountGamificationLevelQuery, Models.GamificationLevel, IGamificationLevelRepository>
    {
        public CountGamificationLevelQueryHandler(IGamificationLevelRepository repository) : base(repository)
        {
        }
    }
}