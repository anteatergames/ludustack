﻿using LuduStack.Domain.Interfaces.Repository;
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
        public GetJobPositionQuery() : base()
        {
        }

        public GetJobPositionQuery(Expression<Func<Models.JobPosition, bool>> where) : base(where)
        {
        }
    }

    public class GetJobPositionQueryHandler : SearchBaseQueryHandler<GetJobPositionQuery, Models.JobPosition, IJobPositionRepository>
    {
        public GetJobPositionQueryHandler(IJobPositionRepository repository) : base(repository)
        {
        }

        public async Task<IEnumerable<Models.JobPosition>> Handle(GetJobPositionQuery request, CancellationToken cancellationToken)
        {
            var all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}