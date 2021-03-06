﻿using LuduStack.Domain.Models;
using MongoDB.Bson.Serialization;

namespace LuduStack.Infra.Data.MongoDb.Maps
{
    public static class TranslationMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<Localization>(map =>
            {
                map.AutoMap();
            });

            BsonClassMap.RegisterClassMap<LocalizationTerm>(map =>
            {
                map.AutoMap();
            });

            BsonClassMap.RegisterClassMap<LocalizationEntry>(map =>
            {
                map.AutoMap();
            });
        }
    }
}