using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<List<T>> ToMongoListAsync<T>(this IQueryable<T> mongoQueryOnly)
        {
            return await ((IMongoQueryable<T>)mongoQueryOnly).ToListAsync();
        }
    }
}