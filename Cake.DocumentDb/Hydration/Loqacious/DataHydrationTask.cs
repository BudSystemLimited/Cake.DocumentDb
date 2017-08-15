using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Hydration.Loqacious
{
    public class DataHydrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Func<ICakeLog, dynamic, object> DocumentCreator { get; }
        public Func<ICakeLog, DocumentDbMigrationSettings, IList<dynamic>> DataProvider { get; }

        public DataHydrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Func<ICakeLog, dynamic, object> documentCreator,
            Func<ICakeLog, DocumentDbMigrationSettings, IList<dynamic>> dataProvider)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Cannot be null or empty", nameof(description));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Cannot be null or empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null or empty", nameof(collectionName));

            if (documentCreator == null)
                throw new ArgumentException("Cannot be null or empty", nameof(documentCreator));

            if (dataProvider == null)
                throw new ArgumentException("Cannot be null or empty", nameof(dataProvider));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            DocumentCreator = documentCreator;
            DataProvider = dataProvider;
        }
    }
}
