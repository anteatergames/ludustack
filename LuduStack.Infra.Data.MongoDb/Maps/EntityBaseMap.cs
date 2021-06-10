using LuduStack.Domain.Core.Models;
using LuduStack.Domain.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace LuduStack.Infra.Data.MongoDb.Maps
{
    public static class EntityBaseMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<Entity>(map =>
            {
                map.AutoMap();
                map.SetIsRootClass(true);
                map.MapIdMember(x => x.Id).SetIdGenerator(GuidGenerator.Instance);
                map.MapMember(x => x.CreateDate).SetDefaultValue(DateTime.Now);
                map.AddKnownType(typeof(UserProfile));
                map.AddKnownType(typeof(Game));
                map.AddKnownType(typeof(UserContent));
                map.AddKnownType(typeof(Gamification));
                map.AddKnownType(typeof(GamificationLevel));
                map.AddKnownType(typeof(GamificationAction));
                map.AddKnownType(typeof(UserContent));
                map.AddKnownType(typeof(Poll));
                map.AddKnownType(typeof(PollOption));
                map.AddKnownType(typeof(PollVote));
                map.AddKnownType(typeof(BillRate));
                map.AddKnownType(typeof(ForumCategory));
                map.AddKnownType(typeof(ForumPost));
            });
        }
    }
}