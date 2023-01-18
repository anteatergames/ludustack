using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetByProductCodes(IEnumerable<string> productCodes)
        {
            FindOptions<Order> findOptions = new FindOptions<Order>();

            FilterDefinition<Order> filter = new ExpressionFilterDefinition<Order>(x => x.Items.Any(y => productCodes.Contains(y.Code)));

            List<Order> orders = await (await DbSet.FindAsync(filter, findOptions)).ToListAsync();

            return orders;
        }
    }
}