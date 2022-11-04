using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class StorePartnershipRepository : BaseRepository<StorePartnership>, IStorePartnershipRepository
    {
        public StorePartnershipRepository(IMongoContext context) : base(context)
        {
        }

        public Task<StorePartnership> GetByPartner(Guid partnerUserId)
        {
            var result = DbSet.AsQueryable().Where(x => x.PartnerUserId == partnerUserId).FirstOrDefault();

            return Task.FromResult(result);
        }
    }
}