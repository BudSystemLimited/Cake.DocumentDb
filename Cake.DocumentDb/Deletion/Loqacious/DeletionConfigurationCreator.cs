using System;

namespace Cake.DocumentDb.Deletion.Loqacious
{
    public class DeletionConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Func<dynamic, object> partitionKeyAccessor;
        private Func<dynamic, bool> filter;

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

        public void PartitionKeyAccessor(Func<dynamic, object> setPartitionKeyAccessor)
        {
            partitionKeyAccessor = setPartitionKeyAccessor;
        }

        public void Filter(Func<dynamic, bool> setFilter)
        {
            filter = setFilter;
        }

        internal DeletionTask DeletionTask => new DeletionTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            partitionKeyAccessor,
            filter);
    }
}
