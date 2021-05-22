using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.JobPosition
{
    public class GetJobPositionIdsQuery : GetIdsBaseQuery<Models.JobPosition>
    {
        public GetJobPositionIdsQuery()
        {
        }

        public GetJobPositionIdsQuery(Expression<Func<Models.JobPosition, bool>> where) : base(where)
        {
        }
    }

    public class GetJobPositionIdsQueryHandler : GetIdsBaseQueryHandler<GetJobPositionIdsQuery, Models.JobPosition, IJobPositionRepository>
    {
        public GetJobPositionIdsQueryHandler(IJobPositionRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Guid>> Handle(GetJobPositionIdsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}