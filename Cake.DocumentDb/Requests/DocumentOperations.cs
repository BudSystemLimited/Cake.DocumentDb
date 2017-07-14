using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Factories;
using Cake.DocumentDb.Migration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;

namespace Cake.DocumentDb.Requests
{
    public class DocumentOperations
    {
        private readonly ICakeContext context;
        private readonly IReliableReadWriteDocumentClient client;
        private readonly CollectionOperations collectionOperations;

        public DocumentOperations(ConnectionSettings settings, ICakeContext context)
            : this(new ClientFactory(settings, context), new CollectionOperations(settings, context))
        {
            this.context = context;
        }

        public DocumentOperations(ClientFactory clientFactory, CollectionOperations collectionOperations)
        {
            this.client = clientFactory.GetClient();
            this.collectionOperations = collectionOperations;
        }

        public VersionInfo GetVersionInfo(
            string database,
            string collection,
            string partitionKeyPath = null,
            int? throughput = null)
        {
            var collectionResource = collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                database,
                "VersionInfo",
                partitionKeyPath,
                throughput);

            var requestOptions = new RequestOptions();

            if (!string.IsNullOrWhiteSpace(partitionKeyPath))
                requestOptions.PartitionKey = new PartitionKey(partitionKeyPath);

            var versionInfo = client.CreateDocumentQuery<VersionInfo>(collectionResource.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(d => d.Id == collection)
                .AsEnumerable()
                .FirstOrDefault();

            return (dynamic)versionInfo ?? new VersionInfo { Id = collection };
        }

        public void UpsertVersionInfo(
            string database, 
            VersionInfo versionInfo)
        {
            UpsertDocument(
                database,
                "VersionInfo",
                versionInfo);
        }

        public IList<dynamic> GetDocuments(
            string database,
            string collection,
            string partitionKeyPath = null,
            int? throughput = null)
        {
            var collectionResource = collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                database,
                collection,
                partitionKeyPath,
                throughput);

            var requestOptions = new RequestOptions();

            if (!string.IsNullOrWhiteSpace(partitionKeyPath))
                requestOptions.PartitionKey = new PartitionKey(partitionKeyPath);

            return client.CreateDocumentQuery<dynamic>(collectionResource.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true })
                .AsEnumerable()
                .ToList();
        }

        public Document UpsertDocument(
            string database,
            string collection,
            object document,
            string partitionKeyPath = null,
            int? throughput = null)
        {
            var collectionResource = collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                database,
                collection,
                partitionKeyPath,
                throughput);

            var requestOptions = new RequestOptions();

            if (!string.IsNullOrWhiteSpace(partitionKeyPath))
                requestOptions.PartitionKey = new PartitionKey(partitionKeyPath);

            return client.UpsertDocumentAsync(
                collectionResource.SelfLink,
                document,
                requestOptions,
                true).Result;
        }

        public void DeleteDocuments(
            string database,
            string collection,
            Func<dynamic, bool> filter, 
            string partitionKeyPath = null,
            Func<dynamic, object> partitionKeyAccessor = null,
            int? throughput = null)
        {
            var collectionResource = collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                database,
                collection,
                partitionKeyPath,
                throughput);

            context.Log.Information("Before Query");

            var documents = client.CreateDocumentQuery<dynamic>(collectionResource.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(filter)
                .AsEnumerable()
                .ToList();

            foreach (var document in documents)
            {
                var requestOptions = new RequestOptions();

                context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Removing document from database: {database} collection: {collection} with id: {document.id}");

                if (partitionKeyAccessor != null)
                {
                    var key = partitionKeyAccessor(document);
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Partition key of {key}");
                    requestOptions.PartitionKey = new PartitionKey(key);
                }

                var result = client.DeleteDocumentAsync(document._self, requestOptions).Result;
            }
        }
    }
}
