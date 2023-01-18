using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class GetStorePartnershipByIdQuery : GetByIdBaseQuery<Models.StorePartnership>
    {
        public GetStorePartnershipByIdQuery(Guid id) : base(id)
        {
        }
    }

    public class GetStorePartnershipByIdQueryHandler : GetByIdBaseQueryHandler<GetStorePartnershipByIdQuery, Models.StorePartnership, IStorePartnershipRepository>
    {
        public GetStorePartnershipByIdQueryHandler(IStorePartnershipRepository repository) : base(repository)
        {
        }

        public override async Task<Models.StorePartnership> Handle(GetStorePartnershipByIdQuery request, CancellationToken cancellationToken)
        {
            Models.StorePartnership obj = await repository.GetById(request.Id);

            if (obj != null)
            {
                obj.CreateDate = obj.CreateDate.ToLocalTime();

                obj.PublishDate = obj.PublishDate.ToLocalTime();
            }

            return obj;
        }
    }
}