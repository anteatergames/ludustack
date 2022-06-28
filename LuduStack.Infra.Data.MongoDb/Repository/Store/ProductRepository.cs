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
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(IMongoContext context) : base(context)
        {
        }

        public Task<List<Product>> GetByOwner(Guid ownerUserId)
        {
            var result = DbSet.AsQueryable().Where(x => x.Owners.Any(x => x.UserId == ownerUserId)).ToList();

            return Task.FromResult(result);
        }
    }
}