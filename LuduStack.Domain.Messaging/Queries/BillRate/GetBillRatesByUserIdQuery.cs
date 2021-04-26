using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.BillRate
{
    public class GetBillRatesByUserIdQuery : Query<IEnumerable<Models.BillRate>>
    {
        public Guid UserId { get; }

        public GetBillRatesByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetBillRatesByUserIdQueryHandler : QueryHandler, IRequestHandler<GetBillRatesByUserIdQuery, IEnumerable<Models.BillRate>>
    {
        protected readonly IBillRateRepository billRateRepository;

        public GetBillRatesByUserIdQueryHandler(IBillRateRepository billRateRepository)
        {
            this.billRateRepository = billRateRepository;
        }

        public async Task<IEnumerable<Models.BillRate>> Handle(GetBillRatesByUserIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Models.BillRate> objs = await billRateRepository.GetByUserId(request.UserId);

            return objs;
        }
    }
}