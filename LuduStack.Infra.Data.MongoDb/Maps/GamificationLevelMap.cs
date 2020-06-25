using LuduStack.Domain.Models;
using MongoDB.Bson.Serialization;

namespace LuduStack.Infra.Data.MongoDb.Maps
{
    public static class GamificationLevelMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<GamificationLevel>(map =>
            {
                map.AutoMap();
            });
        }
    }
}