using LuduStack.Domain.Models;
using MongoDB.Bson.Serialization;

namespace LuduStack.Infra.Data.MongoDb.Maps
{
    public static class PollMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<Poll>(map =>
            {
                map.AutoMap();
            });

            BsonClassMap.RegisterClassMap<PollOption>(map =>
            {
                map.AutoMap();
            });

            BsonClassMap.RegisterClassMap<PollVote>(map =>
            {
                map.AutoMap();
            });
        }
    }
}