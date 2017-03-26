using System;
using Cake.Core;
using Cake.DocumentDb.Factories;
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
