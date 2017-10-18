using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    public class DataMigrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Action<ICakeLog, JObject, IDictionary<string, IList<JObject>>> map;
        private Func<ICakeLog, DocumentDbMigrationSettings, IDictionary<string, IList<JObject>>> dataProvider;

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

        public void Map(Action<ICakeLog, JObject, IDictionary<string, IList<JObject>>> setMap)
        {
            map = setMap;
        }

        public void DataProvider(Func<ICakeLog, DocumentDbMigrationSettings, IDictionary<string, IList<JObject>>> setDataProvider)
        {
            dataProvider = setDataProvider;
        }

        internal DataMigrationTask MigrationTask => new DataMigrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            map,
            dataProvider);
    }
}