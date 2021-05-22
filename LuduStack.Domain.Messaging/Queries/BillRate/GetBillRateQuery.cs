using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.BillRate
{
    public class GetBillRateQuery : GetBaseQuery<Models.BillRate>
    {
        public GetBillRateQuery()
        {
        }

        public GetBillRateQuery(Expression<Func<Models.BillRate, bool>> where) : base(where)
        {
        }
    }

    public class GetBillRateQueryHandler : GetBaseQueryHandler<GetBillRateQuery, Models.BillRate, IBillRateRepository>
    {
        public GetBillRateQueryHandler(IBillRateRepository repository) : base(repository)
        {
        }

        public new async Task<IEnumerable<Models.BillRate>> Handle(GetBillRateQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.BillRate> all = await base.Handle(request, cancellationToken);

            return all;
        }
    }
}