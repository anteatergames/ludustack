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
    public class GetProductQuery : GetBaseQuery<Models.Product>
    {
        public GetProductQuery()
        {
        }

        public GetProductQuery(Expression<Func<Models.Product, bool>> where) : base(where)
        {
        }
    }

    public class GetProductQueryHandler : QueryHandler, IRequestHandler<GetProductQuery, IEnumerable<Models.Product>>
    {
        protected readonly IProductRepository repository;

        public GetProductQueryHandler(IProductRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Models.Product>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.Product> result;

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