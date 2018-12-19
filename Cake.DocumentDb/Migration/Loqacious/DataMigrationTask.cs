using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    internal class DataMigrationTask : IMigrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Expression<Func<JObject, bool>> Filter => doc => true;

        public Action<ICakeLog, JObject, IDictionary<string, IList<JObject>>> Map { get; }
        public Func<ICakeLog, DocumentDbMigrationSettings, IDictionary<string, IList<JObject>>> DataProvider { get; }

        public DataMigrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Action<ICakeLog, JObject, IDictionary<string, IList<JObject>>> map,
            Func<ICakeLog, DocumentDbMigrationSettings, IDictionary<string, IList<JObject>>> dataProvider)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Cannot be null or empty", nameof(description));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Cannot be null or empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null or empty", nameof(collectionName));

            if (map == null)
                throw new ArgumentException("Cannot be null or empty", nameof(map));

            if (dataProvider == null)
                throw new ArgumentException("Cannot be null or empty", nameof(dataProvider));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            Map = map;
            DataProvider = dataProvider;
        }
    }
}