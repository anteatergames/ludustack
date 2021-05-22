using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.JobPosition
{
    public class GetJobPositionQuery : GetBaseQuery<Models.JobPosition>
    {
        public GetJobPositionQuery()
        {
        }

        public GetJobPositionQuery(Expression<Func<Models.JobPosition, bool>> where) : base(where)
        {
        }
    }

    public class GetJobPositionQueryHandler : GetBaseQueryHandler<GetJobPositionQuery, Models.JobPosition, IJobPositionRepository>
    {
        public GetJobPositionQueryHandler(IJobPositionRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.JobPosition>> Handle(GetJobPositionQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.JobPosition> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}