using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Hydration.Loqacious
{
    public class DataHydrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Func<ICakeLog, dynamic, object> documentCreator;
        private Func<DocumentDbMigrationSettings, IList<dynamic>> dataProvider;

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

        public void DataProvider(Func<DocumentDbMigrationSettings, IList<dynamic>> setDataProvider)
        {
            dataProvider = setDataProvider;
        }

        internal DataHydrationTask MigrationTask => new DataHydrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            documentCreator,
            dataProvider);
    }
}
