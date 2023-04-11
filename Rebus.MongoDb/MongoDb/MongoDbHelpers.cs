using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Rebus.MongoDb
{
    /// <summary>
    /// Helper functions to interact with mongodb.
    /// </summary>
    public static class MongoDbHelpers
    {
        /// <summary>
        /// Return true if the actual database pointed by the client is capable of using 
        /// change stream (replicaset and version 3.6+).
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool SupportsChangeStreams(IMongoClient client)
        {
            try
            {
                var adminDatabase = client.GetDatabase("admin");

                // Check the server version
                var buildInfoCommand = new BsonDocumentCommand<BsonDocument>(new BsonDocument { { "buildInfo", 1 } });
                var buildInfo = adminDatabase.RunCommand(buildInfoCommand);
                var versionString = buildInfo.GetValue("version").AsString;
                var version = Version.Parse(versionString);

                // Check the deployment type
                var isMasterCommand = new BsonDocumentCommand<BsonDocument>(new BsonDocument { { "isMaster", 1 } });
                var isMaster = adminDatabase.RunCommand(isMasterCommand);
                var isReplicaSet = isMaster.Contains("setName") || isMaster.Contains("isreplicaset");
                var isSharded = isMaster.GetValue("msg", "").AsString == "isdbgrid";

                // Change Streams are supported in MongoDB 3.6+ and on replica sets or sharded clusters
                return version >= new Version(3, 6) && (isReplicaSet || isSharded);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
