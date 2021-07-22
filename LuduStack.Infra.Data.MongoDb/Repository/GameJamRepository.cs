using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GameJamRepository : BaseRepository<GameJam>, IGameJamRepository
    {
        public GameJamRepository(IMongoContext context) : base(context)
        {
        }

        public Task<IQueryable<GameJamListItem>> GetList()
        {
            IQueryable<GameJamListItem> obj = DbSet.AsQueryable().Where(x => !x.Unlisted).Select(x => new GameJamListItem
            {
                Id = x.Id,
                UserId = x.UserId,
                CreateDate = x.CreateDate,
                Type = x.Type,
                Voters = x.Voters,
                Handler = x.Handler,
                Name = x.Name,
                FeaturedImage = x.FeaturedImage,
                TimeZone = x.TimeZone,
                StartDate = x.StartDate,
                EntryDeadline = x.EntryDeadline,
                VotingEndDate = x.VotingEndDate,
                ResultDate = x.ResultDate,
                ShortDescription = x.ShortDescription,
                Language = x.Language
            });

            return Task.FromResult(obj);
        }
    }
}