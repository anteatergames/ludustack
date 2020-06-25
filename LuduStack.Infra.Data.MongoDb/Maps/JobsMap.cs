using LuduStack.Domain.Models;
using MongoDB.Bson.Serialization;

namespace LuduStack.Infra.Data.MongoDb.Maps
{
    public static class JobsMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<JobPosition>(map =>
            {
                map.AutoMap();
            });

            BsonClassMap.RegisterClassMap<JobApplicant>(map =>
            {
                map.AutoMap();
            });
        }
    }
}