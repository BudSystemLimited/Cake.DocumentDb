using System;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;

namespace Cake.DocumentDb.Operations
{
    public class Migrations
    {
        public static void Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            RunMigrations(context, assembly, settings);
            RunSqlMigrations(context, assembly, settings);
        }

        private static void RunMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migrations");

            var migrations = InstanceProvider.GetInstances<IDocumentMigration>(assembly, settings.Profile);

            var operation = new DocumentOperations(settings.Connection, context);

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

        private static void RunSqlMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migrations");

            var migrations = InstanceProvider.GetInstances<ISqlDocumentMigration>(assembly, settings.Profile);

            var operation = new DocumentOperations(settings.Connection, context);

            foreach (var migration in migrations)
            {
                migration.Log = context.Log;
                migration.ConnectionDetails = settings.SqlConnection;

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
