using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class JobPositionRepository : BaseRepository<JobPosition>, IJobPositionRepository
    {
        public JobPositionRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<bool> AddApplicant(Guid jobPositionId, JobApplicant applicant)
        {
            FilterDefinition<JobPosition> filter = Builders<JobPosition>.Filter.Where(x => x.Id == jobPositionId);
            UpdateDefinition<JobPosition> add = Builders<JobPosition>.Update.AddToSet(c => c.Applicants, applicant);

            UpdateResult result = await DbSet.UpdateOneAsync(filter, add);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }
    }
}