using System;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Interfaces;
using Cake.DocumentDb.Requests;
using LogLevel = Cake.Core.Diagnostics.LogLevel;
using Verbosity = Cake.Core.Diagnostics.Verbosity;

namespace Cake.DocumentDb
{
    [CakeAliasCategory("DocumentDatabase")]
    public static class DocumentDatabaseAlias
    {
        [CakeMethodAlias]
        public static void RunDocumentSeed(this ICakeContext context, string assembly, DocumentConnectionSettings settings, string profile)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Using Profile: " + profile);

            RunDatabaseCreations(assembly, settings, profile, context);
            RunDatabaseCollectionCreations(assembly, settings, profile, context);
            RunSeeds(assembly, settings, profile, context);
        }

        private static void RunDatabaseCreations(string assembly, DocumentConnectionSettings settings, string profile, ICakeContext context)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Database Creations");

            var databases = (from t in Assembly.LoadFile(assembly).GetTypes()
                        where t.GetInterfaces().Contains(typeof(ICreateDocumentDatabase)) && t.GetConstructor(Type.EmptyTypes) != null && (t.CustomAttributes.All(a => a.AttributeType != typeof(ProfileAttribute)) || t.GetCustomAttribute<ProfileAttribute>().Profiles.Contains(profile))
                             select Activator.CreateInstance(t) as ICreateDocumentDatabase)
                        .ToList();

            var operation = new DatabaseOperations(settings, context);

            foreach (var database in databases)
            {
                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Creating Database: " + database.Name);
                operation.GetOrCreateDatabaseIfNotExists(database.Name);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Database Creations");
        }

        private static void RunDatabaseCollectionCreations(string assembly, DocumentConnectionSettings settings, string profile, ICakeContext context)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Database Collection Creations");

            var collections = (from t in Assembly.LoadFile(assembly).GetTypes()
                            where t.GetInterfaces().Contains(typeof(ICreateDocumentDatabaseCollection)) && t.GetConstructor(Type.EmptyTypes) != null && (t.CustomAttributes.All(a => a.AttributeType != typeof(ProfileAttribute)) || t.GetCustomAttribute<ProfileAttribute>().Profiles.Contains(profile))
                               select Activator.CreateInstance(t) as ICreateDocumentDatabaseCollection)
                            .ToList();

            var operation = new CollectionOperations(settings, context);

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

        private static void RunSeeds(string assembly, DocumentConnectionSettings settings, string profile, ICakeContext context)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Seeds");

            var seeds = (from t in Assembly.LoadFile(assembly).GetTypes()
                        where t.GetInterfaces().Contains(typeof(ISeedDocument)) && t.GetConstructor(Type.EmptyTypes) != null && (t.CustomAttributes.All(a => a.AttributeType != typeof(ProfileAttribute)) || t.GetCustomAttribute<ProfileAttribute>().Profiles.Contains(profile))
                         select Activator.CreateInstance(t) as ISeedDocument)
                        .ToList();

            var operation = new DocumentOperations(settings, context);

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
