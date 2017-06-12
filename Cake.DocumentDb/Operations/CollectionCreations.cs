using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Collection;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;

namespace Cake.DocumentDb.Operations
{
    public class CollectionCreations
    {
        public static void Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Database Collection Creations");

            var collections = InstanceProvider.GetInstances<ICreateDocumentDatabaseCollection>(assembly, settings.Profile);

            var operation = new CollectionOperations(settings.Connection, context);

            foreach (var collection in collections)
            {
                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Creating Database Collection: " + collection.CollectionName + " On Database: " + collection.DatabaseName);

                operation.GetOrCreateDocumentCollectionIfNotExists(
                    collection.DatabaseName,
                    collection.CollectionName,
                    collection.PartitionKey,
                    collection.Throughput);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Database Collection Creations");
        }
    }
}
