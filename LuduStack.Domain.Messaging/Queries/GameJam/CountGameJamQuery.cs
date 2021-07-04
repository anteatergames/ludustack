using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class CountGameJamQuery : CountBaseQuery<Models.GameJam>
    {
        public CountGameJamQuery()
        {
        }

        public CountGameJamQuery(Expression<Func<Models.GameJam, bool>> where) : base(where)
        {
        }
    }

    public class CountGameJamQueryHandler : CountBaseQueryHandler<CountGameJamQuery, Models.GameJam, IGameJamRepository>
    {
        public CountGameJamQueryHandler(IGameJamRepository repository) : base(repository)
        {
        }
    }
}