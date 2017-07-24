using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    public class SqlMigrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Action<ICakeLog, JObject, IDictionary<string, IList<dynamic>>> map;
        private SqlStatement[] sqlStatements;
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

        public void Map(Action<ICakeLog, JObject, IDictionary<string, IList<dynamic>>> setMap)
        {
            map = setMap;
        }

        public void SqlStatements(SqlStatement[] setSqlStatements)
        {
            sqlStatements = setSqlStatements;
        }

        public void Filter(Func<dynamic, bool> setFilter)
        {
            filter = setFilter;
        }

        internal SqlMigrationTask MigrationTask => new SqlMigrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            map,
            sqlStatements,
            filter);
    }
}