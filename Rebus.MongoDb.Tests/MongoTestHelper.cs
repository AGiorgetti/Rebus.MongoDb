﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Rebus.Tests.Contracts;

namespace Rebus.MongoDb.Tests
{
    public class MongoTestHelper
    {
        public const string TestCategory = "mongodb";

        public static MongoUrl GetUrl()
        {
            var suffix = TestConfig.Suffix;

            var databaseName = $"rebus2_test_{suffix}".TrimEnd('_');

            var builder = new MongoUrlBuilder(Environment.GetEnvironmentVariable("REBUS_MONGODB"));
            builder.DatabaseName = databaseName;
            var mongoUrl = builder.ToMongoUrl();

            Console.WriteLine("Using MongoDB {0}", mongoUrl);

            return mongoUrl;
        }

        internal static void DropCollection(string collectionName)
        {
            GetMongoDatabase().DropCollection(collectionName);
        }

        public static IMongoDatabase GetMongoDatabase()
        {
            return GetMongoDatabase(GetMongoClient());
        }

        public static void DropMongoDatabase()
        {
            GetMongoClient().DropDatabaseAsync(GetUrl().DatabaseName).Wait();
        }

        static IMongoDatabase GetMongoDatabase(IMongoClient mongoClient)
        {
            var url = GetUrl();
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            var settings = new MongoDatabaseSettings
            {
                WriteConcern = WriteConcern.Acknowledged
            };
            return mongoClient.GetDatabase(url.DatabaseName, settings);
        }

        static IMongoClient GetMongoClient()
        {
            var url = GetUrl();

            return new MongoClient(url);
        }
    }
}