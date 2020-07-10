using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class ShortUrlRepository : BaseRepository<ShortUrl>, IShortUrlRepository
    {
        public ShortUrlRepository(IMongoContext context) : base(context)
        {
        }
    }
}
