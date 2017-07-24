using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Migration.Loqacious
{
    internal class DocumentMigrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Action<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>> Map { get; }
        public DocumentStatement[] DocumentStatements { get; }
        public Func<dynamic, bool> Filter { get; }

        public DocumentMigrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Action<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>> map,
            DocumentStatement[] documentStatements,
            Func<dynamic, bool> filter)
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