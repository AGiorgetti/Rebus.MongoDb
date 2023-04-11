using MongoDB.Driver;
using Rebus.MongoDb.Transport;

namespace Rebus.Config
{
    /// <summary>
    /// Describes options used to configure the <seealso cref="MongoDbTransport"/>
    /// </summary>
    public class MongoDbTransportOptions
    {
        /// <summary>
        /// Creates an instance of the transport connecting via <paramref name="connectionString"/>
        /// </summary>
        public MongoDbTransportOptions(
            string connectionString) : this(new MongoUrl(connectionString))
        {
        }

        /// <summary>
        /// Creates an instance of transport connection
        /// </summary>
        /// <param name="connectionString"></param>
        public MongoDbTransportOptions(MongoUrl connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Set input queue name value
        /// </summary>
        /// <param name="inputQueueName"></param>
        /// <returns></returns>
        public MongoDbTransportOptions SetInputQueueName(string inputQueueName)
        {
            this.InputQueueName = inputQueueName;
            return this;
        }

        /// <summary>
        /// Connection string.
        /// </summary>
        public MongoUrl ConnectionString { get; internal set; }

        /// <summary>
        /// Name of the input queue to process. If <c>null</c> or whitespace the transport will be configured in one way mode (send only)
        /// </summary>
        public string InputQueueName { get; internal set; }

        /// <summary>
        /// If <c>false</c> collections will not be created and must be created outside of Rebus
        /// </summary>
        public bool EnsureCollectionsAreCreated { get; internal set; } = true;

        /// <summary>
        /// If <c>true</c> the transport is configured in one way mode
        /// </summary>
        public bool IsOneWayQueue => InputQueueName == null;

        /// <summary>
        /// If true and database supports change streams, instead of polling for new message the client
        /// will create a change stream to know when a new message is available. Check also 
        /// <see cref="MaxWaitInMillisecondsWhenChangeStreamIsEnabled"/>.
        /// </summary>
        public bool UseChangeStream { get; set; }

        /// <summary>
        /// If <see cref="UseChangeStream"/> is true, database is not polled, but to avoid the risk of
        /// missing a message, we do not trust change stream if more than this amount of time has passed
        /// </summary>
        public int MaxWaitInMillisecondsWhenChangeStreamIsEnabled { get; set; } = 20000;
    }
}
