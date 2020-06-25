using LuduStack.Domain.Models;
using MongoDB.Bson.Serialization;

namespace LuduStack.Infra.Data.MongoDb.Maps
{
    public static class UserContentMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<UserContent>(map =>
            {
                map.AutoMap();
                map.MapMember(x => x.Content).SetIsRequired(true);
            });
        }
    }
}