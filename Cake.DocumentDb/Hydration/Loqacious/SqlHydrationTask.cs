using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.Hydration.Loqacious
{
    internal class SqlHydrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Func<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>, object> DocumentCreator { get; }
        public SqlStatement SqlStatement { get; }
        public SqlStatement[] AdditionalSqlStatements { get; }

        public SqlHydrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Func<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>, object> documentCreator,
            SqlStatement sqlStatement,
            SqlStatement[] additionalSqlStatements = null)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Cannot be null or empty", nameof(description));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Cannot be null or empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null or empty", nameof(collectionName));

            if (documentCreator == null)
                throw new ArgumentException("Cannot be null or empty", nameof(documentCreator));

            if (sqlStatement == null)
                throw new ArgumentException("Cannot be null or empty", nameof(sqlStatement));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            DocumentCreator = documentCreator;
            SqlStatement = sqlStatement;
            AdditionalSqlStatements = additionalSqlStatements;
        }
    }
}