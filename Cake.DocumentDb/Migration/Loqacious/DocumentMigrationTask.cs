using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    internal class DocumentMigrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Action<ICakeLog, JObject, IDictionary<string, IList<JObject>>> Map { get; }
        public DocumentStatement[] DocumentStatements { get; }
        public Func<JObject, bool> Filter { get; }

        public DocumentMigrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Action<ICakeLog, JObject, IDictionary<string, IList<JObject>>> map,
            DocumentStatement[] documentStatements,
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

            if (documentStatements == null || documentStatements.Length == 0)
                throw new ArgumentException("Cannot be null or empty", nameof(documentStatements));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            Map = map;
            DocumentStatements = documentStatements;
            Filter = filter;
        }
    }
}