using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;

namespace LuduStack.Domain.Messaging.Queries.JobPosition
{
    public class CountJobPositionQuery : CountBaseQuery<Models.JobPosition>
    {
    }

    public class CountJobPositionQueryHandler : CountBaseQueryHandler<CountJobPositionQuery, Models.JobPosition, IJobPositionRepository>
    {
        public CountJobPositionQueryHandler(IJobPositionRepository repository) : base(repository)
        {
        }
    }
}