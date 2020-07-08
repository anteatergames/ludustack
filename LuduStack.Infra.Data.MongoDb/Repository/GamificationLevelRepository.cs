using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GamificationLevelRepository : BaseRepository<GamificationLevel>, IGamificationLevelRepository
    {
        public GamificationLevelRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<GamificationLevel> GetByNumber(int levelNumber)
        {
            FilterDefinition<GamificationLevel> filter = Builders<GamificationLevel>.Filter.Eq(x => x.Number, levelNumber);

            GamificationLevel result = await DbSet.Find(filter).FirstOrDefaultAsync();

            if (result == null)
            {
                result = new GamificationLevel
                {
                    Number = 1,
                    XpToAchieve = 99999
                };
            }

            return result;
        }
    }
}