using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class CountStorePartnershipQuery : CountBaseQuery<Models.StorePartnership>
    {
        public CountStorePartnershipQuery()
        {
        }

        public CountStorePartnershipQuery(Expression<Func<Models.StorePartnership, bool>> where) : base(where)
        {
        }
    }

    public class CountStorePartnershipQueryHandler : CountBaseQueryHandler<CountStorePartnershipQuery, Models.StorePartnership, IStorePartnershipRepository>
    {
        public CountStorePartnershipQueryHandler(IStorePartnershipRepository repository) : base(repository)
        {
        }
    }
}