using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GamificationRepository : BaseRepository<Gamification>, IGamificationRepository
    {
        public GamificationRepository(IMongoContext context) : base(context)
        {
        }
    }
}