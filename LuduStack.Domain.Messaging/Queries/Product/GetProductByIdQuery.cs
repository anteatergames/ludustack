using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class GetProductByIdQuery : GetByIdBaseQuery<Models.Product>
    {
        public GetProductByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetProductByIdQueryHandler : GetByIdBaseQueryHandler<GetProductByIdQuery, Models.Product, IProductRepository>
    {
        public GetProductByIdQueryHandler(IProductRepository repository) : base(repository)
        {
        }

        public override async Task<Models.Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            Models.Product obj = await repository.GetById(request.Id);

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }
    }
}