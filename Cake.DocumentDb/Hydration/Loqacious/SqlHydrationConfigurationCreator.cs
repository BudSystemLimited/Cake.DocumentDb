using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Migration;

namespace Cake.DocumentDb.Hydration.Loqacious
{
    public class SqlHydrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Func<ICakeLog, dynamic, object> documentCreator;
        private SqlStatement sqlStatement;

        public void Description(string setDescription)
        {
            description = setDescription;
        }

        public void DatabaseName(string setDatabaseName)
        {
            databaseName = setDatabaseName;
        }

        public void CollectionName(string setCollectionName)
        {
            collectionName = setCollectionName;
        }

        public void PartitionKey(string setPartitionKey)
        {
            partitionKey = setPartitionKey;
        }

        public void DocumentCreator(Func<ICakeLog, dynamic, object> setDocumentCreator)
        {
            documentCreator = setDocumentCreator;
        }

        public void SqlStatement(SqlStatement setSqlStatement)
        {
            sqlStatement = setSqlStatement;
        }

        internal SqlHydrationTask MigrationTask => new SqlHydrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            documentCreator,
            sqlStatement);
    }
}