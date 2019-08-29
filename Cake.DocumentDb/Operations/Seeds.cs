using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Extensions;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;
using Cake.DocumentDb.Seed;

namespace Cake.DocumentDb.Operations
{
    public class Seeds
    {
        public static void Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            try
            {
                RunSeeds(context, assembly, settings);
            }
            catch (Exception exception)
            {
                context.LogExceptionHierarchyAsErrors(exception);
            }
        }

        public static void RunSeeds(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Seeds");

            var seeds = InstanceProvider.GetInstances<ISeedDocument>(assembly, settings.Profile);

            var operation = new DocumentOperations(settings.Connection, context);

            foreach (var seed in seeds)
            {
                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Creating Seed: " + seed.FriendlyName + " On Collection: " + seed.Collection + " On Database: " + seed.Database);

                operation.UpsertDocument(
                    seed.Database,
                    seed.Collection,
                    seed.Document());
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
        }
    }
}
