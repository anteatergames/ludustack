using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class ForumCategoryRepository : BaseRepository<ForumCategory>, IForumCategoryRepository
    {
        public ForumCategoryRepository(IMongoContext context) : base(context)
        {
        }
    }
}