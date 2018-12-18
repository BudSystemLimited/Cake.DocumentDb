using System;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    internal class MigrationTask : IMigrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Action<ICakeLog, JObject> Map { get; }
        public Func<JObject, bool> Filter { get; }

        public MigrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Action<ICakeLog, JObject> map,
            Func<JObject, bool> filter)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Cannot be null or empty", nameof(description));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Cannot be null or empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null or empty", nameof(collectionName));

            if (map == null)
                throw new ArgumentException("Cannot be null or empty", nameof(map));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            Map = map;
            Filter = filter;
        }
    }
}