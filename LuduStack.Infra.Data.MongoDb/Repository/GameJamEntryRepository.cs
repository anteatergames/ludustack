using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GameJamEntryRepository : BaseRepository<GameJamEntry>, IGameJamEntryRepository
    {
        public GameJamEntryRepository(IMongoContext context) : base(context)
        {
        }
    }
}