using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Factories;
using Cake.DocumentDb.Migration;
using Cake.DocumentDb.Migration.Loqacious;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Requests
{
    public class DocumentOperations
    {
        private readonly ICakeContext context;
        private readonly IReliableReadWriteDocumentClient client;
        private readonly IDocumentClient clientOptimisedForWrite;
        private readonly CollectionOperations collectionOperations;

        public DocumentOperations(ConnectionSettings settings, ICakeContext context)
            : this(new ClientFactory(settings, context), new CollectionOperations(settings, context))
        {
            this.context = context;
        }

        public DocumentOperations(ClientFactory clientFactory, CollectionOperations collectionOperations)
        {
            this.client = clientFactory.GetClient();
            this.clientOptimisedForWrite = clientFactory.GetClientOptimisedForWrite();
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

        public IList<JObject> GetDocuments(
            string database,
            string collection,
            Func<JObject, bool> filter = null,
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

            if (filter != null)
            {
                return client.CreateDocumentQuery<JObject>(collectionResource.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true })
                    .Where(filter)
                    .AsEnumerable()
                    .ToList();
            }

            return client.CreateDocumentQuery<JObject>(collectionResource.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true })
                .AsEnumerable()
                .ToList();
        }

        public Document CreateDocument(
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

            var result = client.UpsertDocumentAsync(
                collectionResource.SelfLink,
                document,
                requestOptions,
                true).Result;

            return result;
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

        // https://github.com/Azure/azure-cosmos-dotnet-v2/blob/master/samples/documentdb-benchmark/Program.cs
        internal async Task PerformTask(IMigrationTask task, Action<JObject> mapAction)
        {
            var collectionResource = collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                task.DatabaseName,
                task.CollectionName);

            var documentQuery = client
                .CreateDocumentQuery<JObject>(
                    collectionResource.SelfLink,
                    new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            var isMatch = task.Filter?.Compile() ?? (doc => true);

            var offer = (OfferV2)client.CreateOfferQuery()
                .Where(o => o.ResourceLink == collectionResource.SelfLink)
                .AsEnumerable()
                .FirstOrDefault();

            var taskCount = 5;
            if (offer == null)
            {
                context.Log.Write(Verbosity.Normal, LogLevel.Warning, $"Could not determine current throughput, taskCount defaulted to {taskCount}");
            }
            else
            {
                // set taskCount = 1 for each 100 RUs, minimum 1, maximum 250
                var currentCollectionThroughput = offer.Content.OfferThroughput;
                taskCount = Math.Max(currentCollectionThroughput / 100, 1);
                taskCount = Math.Min(taskCount, 250);

                context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Current throughput is {currentCollectionThroughput}RUs, taskCount set to {taskCount}");
            }

            var upsertTasks = new TaskBuffer(taskCount);
            while (documentQuery.HasMoreResults)
            {
                var documents = await documentQuery.ExecuteNextAsync<JObject>();

                foreach (var document in documents)
                {
                    if (isMatch(document))
                    {
                        mapAction(document);

                        var upsertTask = clientOptimisedForWrite.UpsertDocumentAsync(
                            UriFactory.CreateDocumentCollectionUri(task.DatabaseName, task.CollectionName),
                            document,
                            new RequestOptions(),
                            true);

                        upsertTasks.Add(upsertTask);
                        if (upsertTasks.IsFull())
                            await upsertTasks.ExecuteInParallel();
                    }
                }
            }
            await upsertTasks.ExecuteInParallel();
        }
    }
}
