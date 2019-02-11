using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cake.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Migration.Loqacious
{
    public class DocumentMigrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Action<ICakeLog, JObject, IDictionary<string, IList<JObject>>> map;
        private DocumentStatement[] documentStatements;
        private Expression<Func<JObject, bool>> filter;

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

        public void DocumentStatements(DocumentStatement[] setDocumentStatements)
        {
            documentStatements = setDocumentStatements;
        }

        public void Filter(Expression<Func<JObject, bool>> setFilter)
        {
            filter = setFilter;
        }

        internal DocumentMigrationTask MigrationTask => new DocumentMigrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            map,
            documentStatements,
            filter);
    }
}