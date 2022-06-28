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
    public class GetProductByOwnerQuery : Query<IEnumerable<Models.Product>>
    {
        public Guid OwnerUserId { get; private set; }

        public GetProductByOwnerQuery(Guid ownerUserId)
        {
            OwnerUserId = ownerUserId;
        }
    }

    public class GetProductByOwnerQueryHandler : QueryHandler, IRequestHandler<GetProductByOwnerQuery, IEnumerable<Models.Product>>
    {
        protected readonly IProductRepository productRepository;

        public GetProductByOwnerQueryHandler(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        async Task<IEnumerable<Product>> IRequestHandler<GetProductByOwnerQuery, IEnumerable<Product>>.Handle(GetProductByOwnerQuery request, CancellationToken cancellationToken)
        {
            var obj = await productRepository.GetByOwner(request.OwnerUserId);

            return obj;
        }
    }
}