using System;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Interfaces;
using Cake.DocumentDb.Migrations;
using Cake.DocumentDb.Operations;
using Cake.DocumentDb.Requests;
using LogLevel = Cake.Core.Diagnostics.LogLevel;
using Verbosity = Cake.Core.Diagnostics.Verbosity;

namespace Cake.DocumentDb
{
    [CakeAliasCategory("DocumentDatabase")]
    public static class DocumentDatabaseAlias
    {
        [CakeMethodAlias]
        public static void RunDocumentSeed(this ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Using Profile: " + settings.Profile);

            DatabaseCreations.Run(context, assembly, settings);
            CollectionCreations.Run(context, assembly, settings);
            Seeds.Run(context, assembly, settings);
            RunMigrations(assembly, settings.Connection, settings.Profile, context);
            RunSqlMigrations(assembly, settings.Connection, settings.Profile, context);
        }

        private static void RunMigrations(string assembly, ConnectionSettings settings, string profile, ICakeContext context)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migrations");

            var migrations = (from t in Assembly.LoadFile(assembly).GetTypes()
                              where t.GetInterfaces().Contains(typeof(IDocumentMigration)) && t.GetConstructor(Type.EmptyTypes) != null && (t.CustomAttributes.All(a => a.AttributeType != typeof(ProfileAttribute)) || t.GetCustomAttribute<ProfileAttribute>().Profiles.Contains(profile))
                              select Activator.CreateInstance(t) as IDocumentMigration)
                        .ToList();

            var operation = new DocumentOperations(settings, context);

            foreach (var migration in migrations)
            {
                migration.Log = context.Log;

                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migration: " + migration.Description + " On Collection: " + migration.CollectionName + " On Database: " + migration.DatabaseName);

                var versionInfo = operation.GetVersionInfo(
                    migration.DatabaseName,
                    migration.CollectionName);

                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migrationAttribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Migration: " + migration.Description + " On Collection: " + migration.CollectionName + " On Database: " + migration.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var documents = operation.GetDocuments(
                    migration.DatabaseName,
                    migration.CollectionName);

                foreach (var document in documents)
                {
                    migration.Transform(document);

                    operation.UpsertDocument(
                        migration.DatabaseName,
                        migration.CollectionName,
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
                    migration.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
        }

        private static void RunSqlMigrations(string assembly, ConnectionSettings settings, string profile, ICakeContext context)
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

                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migration: " + migration.Description + " On Collection: " + migration.CollectionName + " On Database: " + migration.DatabaseName);

                var versionInfo = operation.GetVersionInfo(
                    migration.DatabaseName,
                    migration.CollectionName);

                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migrationAttribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Migration: " + migration.Description + " On Collection: " + migration.CollectionName + " On Database: " + migration.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                migration.ExecuteSql();

                var documents = operation.GetDocuments(
                    migration.DatabaseName,
                    migration.CollectionName);

                foreach (var document in documents)
                {
                    migration.Transform(document);

                    operation.UpsertDocument(
                        migration.DatabaseName,
                        migration.CollectionName,
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
                    migration.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
        }
    }
}
