using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GamificationActionRepository : BaseRepository<GamificationAction>, IGamificationActionRepository
    {
        public GamificationActionRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<GamificationAction> GetByAction(PlatformAction action)
        {
            FilterDefinition<GamificationAction> filter = Builders<GamificationAction>.Filter.Eq(x => x.Action, action);

            GamificationAction result = await DbSet.Find(filter).FirstOrDefaultAsync();

            return result;
        }
    }
}