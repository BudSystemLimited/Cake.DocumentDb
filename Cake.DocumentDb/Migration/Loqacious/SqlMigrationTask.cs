using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Migration.Loqacious
{
    internal class SqlMigrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Action<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>> Map { get; }
        public SqlStatement[] SqlStatements { get; }
        public Func<dynamic, bool> Filter { get; }

        public SqlMigrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Action<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>> map,
            SqlStatement[] sqlStatements,
            Func<dynamic, bool> filter)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Cannot be null or empty", nameof(description));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Cannot be null or empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null or empty", nameof(collectionName));

            if (string.IsNullOrWhiteSpace(partitionKey))
                throw new ArgumentException("Cannot be null or empty", nameof(partitionKey));

            if (map == null)
                throw new ArgumentException("Cannot be null or empty", nameof(map));

            if (sqlStatements == null || sqlStatements.Length == 0)
                throw new ArgumentException("Cannot be null or empty", nameof(sqlStatements));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            Map = map;
            SqlStatements = sqlStatements;
            Filter = filter;
        }
    }
}