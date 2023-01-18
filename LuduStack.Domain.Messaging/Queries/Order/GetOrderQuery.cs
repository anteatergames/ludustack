using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class GetOrderQuery : GetBaseQuery<Models.Order>
    {
        public GetOrderQuery()
        {
        }

        public GetOrderQuery(Expression<Func<Models.Order, bool>> where) : base(where)
        {
        }
    }

    public class GetOrderQueryHandler : QueryHandler, IRequestHandler<GetOrderQuery, IEnumerable<Models.Order>>
    {
        protected readonly IOrderRepository repository;

        public GetOrderQueryHandler(IOrderRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Models.Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.Order> result;

            if (request.Where != null)
            {
                result = repository.Get(request.Where);

                if (request.Take > 0)
                {
                    result = result.Take(request.Take);
                }

                return result;
            }
            else
            {
                return await repository.GetAll();
            }
        }
    }
}