using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Factories;
using Cake.DocumentDb.Hydration.Loqacious;
using Cake.DocumentDb.Migration;
using Cake.DocumentDb.Migration.Loqacious;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Requests
{
    public class DocumentOperations
    {
        private readonly WriteSettings writeSettings;
        private readonly ICakeContext context;
        private readonly IDocumentClient client;
        private readonly IDocumentClient clientOptimisedForWrite;
        private readonly CollectionOperations collectionOperations;

        public DocumentOperations(ConnectionSettings settings, ICakeContext context)
            : this(
                settings.Write ?? WriteSettings.Default,
                context,
                new ClientFactory(settings, context),
                new CollectionOperations(settings, context))
        {
        }

        public DocumentOperations(
            WriteSettings writeSettings,
            ICakeContext context,
            ClientFactory clientFactory,
            CollectionOperations collectionOperations)
        {
            this.writeSettings = writeSettings;
            this.context = context;
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

            var versionInfo = client
                .CreateDocumentQuery<VersionInfo>(
                    collectionResource.SelfLink,
                    new FeedOptions
                    {
                        EnableCrossPartitionQuery = true
                    })
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
            QuerySpec querySpec = null,
            Func<JObject, bool> filter = null,
            string partitionKeyPath = null,
            int? throughput = null)
        {
            var collectionResource = collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                database,
                collection,
                partitionKeyPath,
                throughput);

            if (querySpec != null)
            {
                return client
                    .CreateDocumentQuery<JObject>(
                        collectionResource.SelfLink,
                        querySpec.SqlQuerySpec,
                        new FeedOptions
                        {
                            EnableCrossPartitionQuery = true
                        })
                    .AsEnumerable()
                    .ToList();
            }
            if (filter != null)
            {
                return client
                    .CreateDocumentQuery<JObject>(
                        collectionResource.SelfLink,
                        new FeedOptions
                        {
                            EnableCrossPartitionQuery = true
                        })
                    .Where(filter) // This where clause is not being performed in Cosmos!
                    .AsEnumerable()
                    .ToList();
            }
            return client
                .CreateDocumentQuery<JObject>(
                    collectionResource.SelfLink,
                    new FeedOptions
                    {
                        EnableCrossPartitionQuery = true
                    })
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

            return client
                .UpsertDocumentAsync(
                    collectionResource.SelfLink,
                    document,
                    requestOptions,
                    true)
                .Result;
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

            return client
                .UpsertDocumentAsync(
                    collectionResource.SelfLink,
                    document,
                    requestOptions,
                    true)
                .Result;
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
        internal async Task PerformMigrationTask(IMigrationTask task, Action<JObject> mapAction)
        {
            var collectionResource = GetCollectionResource(task.DatabaseName, task.CollectionName);
            var documentQuery = GetDocumentQuery(collectionResource, null);
            var taskCount = GetParallelTaskCount(collectionResource);

            var isMatch = task.Filter?.Compile() ?? (doc => true);
            
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

        internal async Task PerformHydrationTask(DocumentHydrationTask task, Func<JObject, IEnumerable<JObject>> documentsToCreateFunc)
        {
            var sourceCollectionResource = GetCollectionResource(task.DocumentStatement.DatabaseName, task.DocumentStatement.CollectionName);
            var documentQuery = GetDocumentQuery(sourceCollectionResource, task.DocumentStatement.Query);

            var destCollectionResource = GetCollectionResource(task.DatabaseName, task.CollectionName);
            var taskCount = GetParallelTaskCount(destCollectionResource);

            var isMatch = task.DocumentStatement.Filter ?? (doc => true);

            var createTasks = new TaskBuffer(taskCount);

            while (documentQuery.HasMoreResults)
            {
                var documents = await documentQuery.ExecuteNextAsync<JObject>();

                foreach (var document in documents)
                {
                    if (isMatch(document))
                    {
                        var documentsToCreate = documentsToCreateFunc(document);

                        foreach (var docToCreate in documentsToCreate)
                        {
                            var createTask = clientOptimisedForWrite.CreateDocumentAsync(
                                destCollectionResource.SelfLink,
                                docToCreate,
                                new RequestOptions(),
                                true);

                            createTasks.Add(createTask);

                            if (createTasks.IsFull())
                                await createTasks.ExecuteInParallel();
                        }
                    }
                }
            }
            await createTasks.ExecuteInParallel();
        }

        private DocumentCollection GetCollectionResource(string database, string collection)
        {
            return collectionOperations.GetOrCreateDocumentCollectionIfNotExists(
                database,
                collection);
        }

        private IDocumentQuery<JObject> GetDocumentQuery(DocumentCollection collectionResource, QuerySpec querySpec)
        {
            var feedOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = true
            };
            var query = querySpec == null
                ? client.CreateDocumentQuery<JObject>(collectionResource.SelfLink, feedOptions)
                : client.CreateDocumentQuery<JObject>(collectionResource.SelfLink, querySpec.SqlQuerySpec, feedOptions);

            return query.AsDocumentQuery();
        }

        private int GetParallelTaskCount(DocumentCollection collectionResource)
        {
            var offer = (OfferV2)client.CreateOfferQuery()
                .Where(o => o.ResourceLink == collectionResource.SelfLink)
                .AsEnumerable()
                .FirstOrDefault();

            var taskCount = 4;
            if (offer == null)
            {
                context.Log.Write(Verbosity.Normal, LogLevel.Warning, $"Could not determine current throughput, taskCount defaulted to {taskCount}");
            }
            else
            {
                var currentCollectionThroughput = offer.Content.OfferThroughput;
                taskCount = Math.Max((int)(currentCollectionThroughput * writeSettings.ThroughputFactor), writeSettings.MinTaskCount);
                taskCount = Math.Min(taskCount, writeSettings.MaxTaskCount);

                context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Current throughput is {currentCollectionThroughput}RUs, taskCount set to {taskCount}");
            }

            return taskCount;
        }
    }
}
