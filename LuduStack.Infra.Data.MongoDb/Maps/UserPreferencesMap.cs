using LuduStack.Domain.Models;
using MongoDB.Bson.Serialization;

namespace LuduStack.Infra.Data.MongoDb.Maps
{
    public static class UserPreferencesMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<UserPreferences>(map =>
            {
                map.AutoMap();
            });
        }
    }
}