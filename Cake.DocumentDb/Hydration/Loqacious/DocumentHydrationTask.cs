using System;
using System.Collections.Generic;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Migration;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Hydration.Loqacious
{
    internal class DocumentHydrationTask
    {
        public string Description { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string PartitionKey { get; }
        public Func<ICakeLog, JObject, IDictionary<string, IList<JObject>>, JObject> DocumentCreator { get; }
        public DocumentStatement DocumentStatement { get; }
        public DocumentStatement[] AdditionalDocumentStatements { get; }

        public DocumentHydrationTask(
            string description,
            string databaseName,
            string collectionName,
            string partitionKey,
            Func<ICakeLog, JObject, IDictionary<string, IList<JObject>>, JObject> documentCreator,
            DocumentStatement documentStatement,
            DocumentStatement[] additionalDocumentStatements = null)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Cannot be null or empty", nameof(description));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Cannot be null or empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Cannot be null or empty", nameof(collectionName));

            if (documentCreator == null)
                throw new ArgumentException("Cannot be null or empty", nameof(documentCreator));

            if (documentStatement == null)
                throw new ArgumentException("Cannot be null or empty", nameof(documentStatement));

            Description = description;
            DatabaseName = databaseName;
            CollectionName = collectionName;
            PartitionKey = partitionKey;
            DocumentCreator = documentCreator;
            DocumentStatement = documentStatement;
            AdditionalDocumentStatements = additionalDocumentStatements;
        }
    }
}