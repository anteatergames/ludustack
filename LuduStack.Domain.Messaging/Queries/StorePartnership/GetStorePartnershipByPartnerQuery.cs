using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class GetStorePartnershipByPartnerQuery : Query<Models.StorePartnership>
    {
        public Guid PartnerUserId { get; private set; }

        public GetStorePartnershipByPartnerQuery(Guid ownerUserId)
        {
            PartnerUserId = ownerUserId;
        }
    }

    public class GetStorePartnershipByPartnerQueryHandler : QueryHandler, IRequestHandler<GetStorePartnershipByPartnerQuery, Models.StorePartnership>
    {
        protected readonly IStorePartnershipRepository productRepository;

        public GetStorePartnershipByPartnerQueryHandler(IStorePartnershipRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        async Task<StorePartnership> IRequestHandler<GetStorePartnershipByPartnerQuery, StorePartnership>.Handle(GetStorePartnershipByPartnerQuery request, CancellationToken cancellationToken)
        {
            var obj = await productRepository.GetByPartner(request.PartnerUserId);

            obj.FundsAvailable = obj.FundsTotal - obj.FundsWithdrawn;

            return obj;
        }
    }
}