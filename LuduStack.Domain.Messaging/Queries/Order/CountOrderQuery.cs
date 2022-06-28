using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class CountOrderQuery : CountBaseQuery<Models.Order>
    {
        public CountOrderQuery()
        {
        }

        public CountOrderQuery(Expression<Func<Models.Order, bool>> where) : base(where)
        {
        }
    }

    public class CountOrderQueryHandler : CountBaseQueryHandler<CountOrderQuery, Models.Order, IOrderRepository>
    {
        public CountOrderQueryHandler(IOrderRepository repository) : base(repository)
        {
        }
    }
}