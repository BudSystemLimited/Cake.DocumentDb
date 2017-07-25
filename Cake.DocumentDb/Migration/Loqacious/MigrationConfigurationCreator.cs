using System;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    public class MigrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Action<ICakeLog, JObject> map;
        private Func<JObject, bool> filter;

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

        public void Map(Action<ICakeLog, JObject> setMap)
        {
            map = setMap;
        }

        public void Filter(Func<JObject, bool> setFilter)
        {
            filter = setFilter;
        }

        internal MigrationTask MigrationTask => new MigrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            map,
            filter);
    }
}
