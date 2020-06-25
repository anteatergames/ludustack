using MongoDB.Driver;
using System;

namespace LuduStack.Infra.Data.MongoDb
{
    public interface IMongoService
    {
        IMongoCollection<T> GetCollection<T>(Action<IMongoCollection<T>> action);
    }
}