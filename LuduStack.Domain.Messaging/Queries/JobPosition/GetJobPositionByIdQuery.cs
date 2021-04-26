using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.JobPosition
{
    public class GetJobPositionByIdQuery : GetByIdBaseQuery<Models.JobPosition>
    {
        public GetJobPositionByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetJobPositionByIdQueryHandler : GetByIdBaseQueryHandler<GetJobPositionByIdQuery, Models.JobPosition, IJobPositionRepository>
    {
        public GetJobPositionByIdQueryHandler(IJobPositionRepository repository) : base(repository)
        {
        }
    }
}