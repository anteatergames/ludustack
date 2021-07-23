using LuduStack.Infra.Data.MongoDb.Maps;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace LuduStack.Infra.Data.MongoDb
{
    public static class MongoDbPersistence
    {
        public static void Configure()
        {
            ConventionPack conventionPack = new()
            {
                new IgnoreIfDefaultConvention(true),
                new IgnoreExtraElementsConvention(true),
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("LuduStackConventions", conventionPack, t => true);

#pragma warning disable CS0618 // Type or member is obsolete
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3; // this will be removed in a future version of the MongoDB driver: http://mongodb.github.io/mongo-csharp-driver/2.11/reference/bson/guidserialization/guidrepresentationmode/guidrepresentationmode/
#pragma warning restore CS0618 // Type or member is obsolete

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            EntityBaseMap.Configure();
            UserProfileMap.Configure();
            GameMap.Configure();
            UserContentMap.Configure();
            UserPreferencesMap.Configure();
            FeaturedContentMap.Configure();
            GamificationActionMap.Configure();
            GamificationLevelMap.Configure();
            GamificationMap.Configure();
            PollMap.Configure();
            BrainstormMap.Configure();
            TeamMap.Configure();
            JobsMap.Configure();
            TranslationMap.Configure();
        }
    }
}