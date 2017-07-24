using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;

namespace Cake.DocumentDb.Migration.Loqacious
{
    public class DocumentMigrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Action<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>> map;
        private DocumentStatement[] documentStatements;
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

        public void Map(Action<ICakeLog, dynamic, IDictionary<string, IList<dynamic>>> setMap)
        {
            map = setMap;
        }

        public void DocumentStatements(DocumentStatement[] setDocumentStatements)
        {
            documentStatements = setDocumentStatements;
        }

        public void Filter(Func<dynamic, bool> setFilter)
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