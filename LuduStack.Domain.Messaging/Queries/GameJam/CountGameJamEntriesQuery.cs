using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.GameJam
{
    public class CountGameJamEntryQuery : CountBaseQuery<Models.GameJamEntry>
    {
        public CountGameJamEntryQuery()
        {
        }

        public CountGameJamEntryQuery(Expression<Func<Models.GameJamEntry, bool>> where) : base(where)
        {
        }
    }

    public class CountGameJamEntryQueryHandler : CountBaseQueryHandler<CountGameJamEntryQuery, Models.GameJamEntry, IGameJamEntryRepository>
    {
        public CountGameJamEntryQueryHandler(IGameJamEntryRepository repository) : base(repository)
        {
        }
    }
}