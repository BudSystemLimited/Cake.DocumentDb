using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Database;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;

namespace Cake.DocumentDb.Operations
{
    public class DatabaseCreations
    {
        public static void Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Database Creations");

            var databases = InstanceProvider.GetInstances<ICreateDocumentDatabase>(assembly, settings.Profile);

            var operation = new DatabaseOperations(settings.Connection, context);

            foreach (var database in databases)
            {
                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Creating Database: " + database.Name);
                operation.GetOrCreateDatabaseIfNotExists(database.Name);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Database Creations");
        }
    }
}
