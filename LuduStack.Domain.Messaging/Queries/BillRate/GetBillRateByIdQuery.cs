using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;

namespace LuduStack.Domain.Messaging.Queries.BillRate
{
    public class GetBillRateByIdQuery : GetByIdBaseQuery<Models.BillRate>
    {
        public GetBillRateByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetBillRateByIdQueryHandler : GetByIdBaseQueryHandler<GetBillRateByIdQuery, Models.BillRate, IBillRateRepository>
    {
        public GetBillRateByIdQueryHandler(IBillRateRepository repository) : base(repository)
        {
        }
    }
}