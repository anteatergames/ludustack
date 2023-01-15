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
    public class GetStorePartnershipQuery : GetBaseQuery<Models.StorePartnership>
    {
        public GetStorePartnershipQuery()
        {
        }

        public GetStorePartnershipQuery(Expression<Func<Models.StorePartnership, bool>> where) : base(where)
        {
        }
    }

    public class GetStorePartnershipQueryHandler : QueryHandler, IRequestHandler<GetStorePartnershipQuery, IEnumerable<Models.StorePartnership>>
    {
        protected readonly IStorePartnershipRepository repository;

        public GetStorePartnershipQueryHandler(IStorePartnershipRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Models.StorePartnership>> Handle(GetStorePartnershipQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Models.StorePartnership> result;

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