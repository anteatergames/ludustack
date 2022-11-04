using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Base;
using System;
using System.Linq.Expressions;

namespace LuduStack.Domain.Messaging.Queries.Store
{
    public class CountProductQuery : CountBaseQuery<Models.Product>
    {
        public CountProductQuery()
        {
        }

        public CountProductQuery(Expression<Func<Models.Product, bool>> where) : base(where)
        {
        }
    }

    public class CountProductQueryHandler : CountBaseQueryHandler<CountProductQuery, Models.Product, IProductRepository>
    {
        public CountProductQueryHandler(IProductRepository repository) : base(repository)
        {
        }
    }
}