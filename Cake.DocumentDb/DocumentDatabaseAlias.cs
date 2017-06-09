using System;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Interfaces;
using Cake.DocumentDb.Migrations;
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
            RunMigrations(assembly, settings, profile, context);
            //RunSeeds(assembly, settings, profile, context);
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

        private static void RunMigrations(string assembly, DocumentConnectionSettings settings, string profile, ICakeContext context)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migrations");

            var migrations = (from t in Assembly.LoadFile(assembly).GetTypes()
                         where t.GetInterfaces().Contains(typeof(ISqlDocumentMigration)) && t.GetConstructor(Type.EmptyTypes) != null && (t.CustomAttributes.All(a => a.AttributeType != typeof(ProfileAttribute)) || t.GetCustomAttribute<ProfileAttribute>().Profiles.Contains(profile))
                         select Activator.CreateInstance(t) as ISqlDocumentMigration)
                        .ToList();

            var operation = new DocumentOperations(settings, context);

            foreach (var migration in migrations)
            {
                migration.Log = context.Log;
                migration.ConnectionDetails = settings.SqlConnectionDetails;

                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migration: " + migration.Description + " On Collection: " + migration.Collection + " On Database: " + migration.Database);

                var versionInfo = operation.GetVersionInfo(
                    migration.Database,
                    migration.Collection);

                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migrationAttribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Migration: " + migration.Description + " On Collection: " + migration.Collection + " On Database: " + migration.Database + " Has Already Been Executed");
                    continue;
                }

                migration.ExecuteSql();

                var documents = operation.GetDocuments(
                    migration.Database,
                    migration.Collection);

                foreach (var document in documents)
                {
                    migration.Transform(document);

                    operation.UpsertDocument(
                        migration.Database,
                        migration.Collection,
                        document);
                }

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = migration.GetType().Name,
                    Description = migration.Description,
                    Timestamp = migrationAttribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    migration.Database,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
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
