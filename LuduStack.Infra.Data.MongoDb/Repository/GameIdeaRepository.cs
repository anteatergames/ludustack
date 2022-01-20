using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GameIdeaRepository : BaseRepository<GameIdea>, IGameIdeaRepository
    {
        public GameIdeaRepository(IMongoContext context) : base(context)
        {
        }
    }
}