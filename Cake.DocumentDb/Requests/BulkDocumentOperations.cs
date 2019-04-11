using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.DocumentDb.Factories;
using Cake.DocumentDb.Migration.Loqacious;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Requests
{
    // TODO:
    // * Cake dependency problems
    // * Error handling
    // * Newtonsoft dependency problems
    internal class BulkDocumentOperations
    {
        private readonly IReliableReadWriteDocumentClient client;
        private readonly DocumentClient bulkExecutorClient;
        private readonly CollectionOperations collectionOperations;

        public BulkDocumentOperations(
            ConnectionSettings settings,
            ICakeContext context)
            : this(
                new ClientFactory(settings, context),
                new CollectionOperations(settings, context))
        {
        }

        public BulkDocumentOperations(
            ClientFactory clientFactory,
            CollectionOperations collectionOperations)
        {
            this.client = clientFactory.GetClient();
            this.bulkExecutorClient = clientFactory.GetClientForBulkExecutor();
            this.collectionOperations = collectionOperations;
        }

        internal async Task PerformTask(
            IMigrationTask task,
            Action<JObject> mapAction)
        {
            var collectionResource = collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                task.DatabaseName,
                task.CollectionName);
            var documentQuery = client.CreateDocumentQuery<JObject>(
                    collectionResource.SelfLink,
                    new FeedOptions
                    {
                        MaxItemCount = 250,
                        EnableCrossPartitionQuery = true
                    })
                .AsDocumentQuery();
            var isMatch = task.Filter?.Compile() ?? (doc => true);

            // Set retry options high during initialization (default values).
            bulkExecutorClient.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 30;
            bulkExecutorClient.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 9;

            var dataCollection = bulkExecutorClient
                .CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(task.DatabaseName))
                .Where(c => c.Id == task.CollectionName)
                .AsEnumerable()
                .FirstOrDefault();

            var bulkExecutor = new BulkExecutor(bulkExecutorClient, dataCollection);
            await bulkExecutor.InitializeAsync();

            // Set retries to 0 to pass complete control to bulk executor.
            bulkExecutorClient.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 0;
            bulkExecutorClient.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 0;

            while (documentQuery.HasMoreResults)
            {
                var documents = await documentQuery.ExecuteNextAsync<JObject>();

                var documentForBulkImport = new List<JObject>();
                foreach (var document in documents)
                {
                    if (isMatch(document))
                    {
                        mapAction(document); // mutate

                        documentForBulkImport.Add(document);
                    }
                }

                await bulkExecutor.BulkImportAsync(
                    documents: documentForBulkImport,
                    enableUpsert: true,
                    disableAutomaticIdGeneration: true,
                    maxConcurrencyPerPartitionKeyRange: null,
                    maxInMemorySortingBatchSize: null);
            }
        }
    }
}