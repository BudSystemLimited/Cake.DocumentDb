using System;

namespace Cake.DocumentDb.Deletion.Loqacious
{
    internal class DeletionTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Func<dynamic, object> PartitionKeyAccessor;
        public Func<dynamic, bool> Filter { get; }

        public DeletionTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Func<dynamic, object> partitionKeyAccessor,
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

            if (partitionKeyAccessor == null)
                throw new ArgumentException("Cannot be null or empty", nameof(partitionKeyAccessor));

            if (filter == null)
                throw new ArgumentException("Cannot be null or empty", nameof(filter));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            PartitionKeyAccessor = partitionKeyAccessor;
            Filter = filter;
        }
    }
}