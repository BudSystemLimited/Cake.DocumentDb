using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.DocumentDb.Factories;
using Cake.DocumentDb.Migrations;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;

namespace Cake.DocumentDb.Requests
{
    public class DocumentOperations
    {
        private readonly IReliableReadWriteDocumentClient client;
        private readonly CollectionOperations collectionOperations;

        public DocumentOperations(DocumentConnectionSettings settings, ICakeContext context)
            :this(new ClientFactory(settings, context), new CollectionOperations(settings, context))
        { }

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

            return versionInfo ?? new VersionInfo { Id = collection };
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
    }
}
