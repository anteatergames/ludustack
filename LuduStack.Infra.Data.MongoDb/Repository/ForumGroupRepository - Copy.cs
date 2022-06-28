﻿using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class SystemEventRepository : BaseRepository<SystemEvent>, ISystemEventRepository
    {
        public SystemEventRepository(IMongoContext context) : base(context)
        {
        }
    }
}