using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Hydration.Loqacious
{
    public class DocumentHydrationConfigurationCreator
    {
        private string description;
        private string databaseName;
        private string collectionName;
        private string partitionKey;
        private Func<ICakeLog, JObject, IDictionary<string, IList<JObject>>, JObject> documentCreator;
        private DocumentStatement documentStatement;
        private DocumentStatement[] additionalDocumentStatements;

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

        public void DocumentCreator(Func<ICakeLog, JObject, IDictionary<string, IList<JObject>>, JObject> setDocumentCreator)
        {
            documentCreator = setDocumentCreator;
        }

        public void DocumentStatement(DocumentStatement setSqlStatement)
        {
            documentStatement = setSqlStatement;
        }

        public void AdditionalDocumentStatements(DocumentStatement[] setSqlStatements)
        {
            additionalDocumentStatements = setSqlStatements;
        }

        internal DocumentHydrationTask MigrationTask => new DocumentHydrationTask(
            description,
            databaseName,
            collectionName,
            partitionKey,
            documentCreator,
            documentStatement,
            additionalDocumentStatements);
    }
}