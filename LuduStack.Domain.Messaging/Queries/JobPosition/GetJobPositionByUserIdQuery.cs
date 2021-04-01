using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.JobPosition
{
    public class GetJobPositionByUserIdQuery : GetByUserIdBaseQuery<Models.JobPosition>
    {
        public GetJobPositionByUserIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetJobPositionsByUserIdQueryHandler : GetByUserIdBaseQueryHandler<GetJobPositionByUserIdQuery, Models.JobPosition, IJobPositionRepository>
    {
        public GetJobPositionsByUserIdQueryHandler(IJobPositionRepository repository) : base(repository)
        {
        }
    }
}