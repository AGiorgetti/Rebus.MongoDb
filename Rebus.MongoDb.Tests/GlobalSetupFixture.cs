using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Rebus.MongoDb.Tests
{
    [SetUpFixture]
    public class GlobalSetupFixture
    {
        internal static List<String> DatabaseCreated = new List<String>();

        [OneTimeSetUp]
        public void This_is_run_before_ANY_tests() 
        {
            FixSerialzierForDriver2_19();
        }

         private static void FixSerialzierForDriver2_19()
        {
            try
            {
                if (!IsSerializerRegistered(typeof(object)))
                {
                    //After version 2.19 of the driver. https://github.com/mongodb/mongo-csharp-driver/releases/tag/v2.19.0
                    var objectSerializer = new ObjectSerializer(type => ObjectSerializer.AllAllowedTypes(type));
                    BsonSerializer.RegisterSerializer(objectSerializer);
                }
            }
            catch (Exception)
            {
                //ignore errors because the serializer could be already registered.
            }
        }

        internal static bool IsSerializerRegistered(Type type)
        {
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var cacheInfo = typeof(BsonSerializerRegistry).GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException("BsonSerializerRegistry._cache field not found. Internal implementation changed!");
            var cache = cacheInfo.GetValue(serializerRegistry) as ConcurrentDictionary<Type, IBsonSerializer>;
            return cache.ContainsKey(type);
        }
    }
}
