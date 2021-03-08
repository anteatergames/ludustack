using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.JobPosition
{
    public class CountJobPositionQuery : CountBaseQuery<Models.JobPosition>
    {
        public CountJobPositionQuery() : base()
        {
        }
    }
    public class CountJobPositionQueryHandler : CountBaseQueryHandler<CountJobPositionQuery, Models.JobPosition, IJobPositionRepository>
    {
        public CountJobPositionQueryHandler(IJobPositionRepository repository) : base(repository)
        {
        }
    }
}
