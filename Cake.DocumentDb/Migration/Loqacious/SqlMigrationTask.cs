using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    internal class SqlMigrationTask : IMigrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Action<ICakeLog, JObject, IDictionary<string, IList<dynamic>>> Map { get; }
        public SqlStatement[] SqlStatements { get; }
        public Expression<Func<JObject, bool>> Filter { get; }

        public SqlMigrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Action<ICakeLog, JObject, IDictionary<string, IList<dynamic>>> map,
            SqlStatement[] sqlStatements,
            Expression<Func<JObject, bool>> filter)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Cannot be null or empty", nameof(description));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Cannot be null or empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null or empty", nameof(collectionName));

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