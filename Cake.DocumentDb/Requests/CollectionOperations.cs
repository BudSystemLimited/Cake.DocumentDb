﻿using System.Linq;
using Cake.Core;
using Cake.DocumentDb.Factories;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Cake.DocumentDb.Requests
{
    public class CollectionOperations
    {
        private readonly IDocumentClient client;
        private readonly DatabaseOperations databaseOperations;

        public CollectionOperations(ConnectionSettings settings, ICakeContext context)
            :this(new ClientFactory(settings, context), new DatabaseOperations(settings, context))
        { }

        public CollectionOperations(ClientFactory clientFactory, DatabaseOperations databaseOperations)
        {
            this.client = clientFactory.GetClient();
            this.databaseOperations = databaseOperations;
        }

        public DocumentCollection GetOrCreateDocumentCollectionIfNotExists(
            string database,
            string collection,
            string partitionKeyPath = null,
            int? throughput = null)
        {
            var databaseResource = databaseOperations.GetOrCreateDatabaseIfNotExists(database);

            var response = client.CreateDocumentCollectionQuery(databaseResource.SelfLink)
                .Where(c => c.Id == collection)
                .ToArray()
                .FirstOrDefault();

            if (response != null)
                return response;

            response = new DocumentCollection
            {
                Id = collection
            };

            if (!string.IsNullOrWhiteSpace(partitionKeyPath))
                response.PartitionKey.Paths.Add(partitionKeyPath);

            return client.CreateDocumentCollectionAsync(
                databaseResource.SelfLink,
                response,
                new RequestOptions { OfferThroughput = throughput }).Result;
        }
    }
}
