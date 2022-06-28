using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class GetOrderByIdQuery : GetByIdBaseQuery<Models.Order>
    {
        public GetOrderByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetOrderByIdQueryHandler : GetByIdBaseQueryHandler<GetOrderByIdQuery, Models.Order, IOrderRepository>
    {
        public GetOrderByIdQueryHandler(IOrderRepository repository) : base(repository)
        {
        }

        public override async Task<Models.Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            Models.Order obj = await repository.GetById(request.Id);

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }
    }
}