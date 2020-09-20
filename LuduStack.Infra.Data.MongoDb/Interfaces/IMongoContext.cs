using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Interfaces
{
    public interface IMongoContext : IDisposable
    {
        bool HasPendingCommands { get; }

        Task AddCommand(Func<Task> func);

        Task<int> SaveChanges();

        IMongoCollection<T> GetCollection<T>(string name);
    }
}