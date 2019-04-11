using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;
using Dapper;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Operations
{
    public class Migrations
    {
        public static async Task Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            try
            {
                await RunMigrations(context, assembly, settings);
                await RunSqlMigrations(context, assembly, settings);
                await RunDocumentMigrations(context, assembly, settings);
                await RunDataMigrations(context, assembly, settings);
            }
            catch (Exception ex)
            {
                context.Log.Error(ex.Message, ex);
                context.Log.Error(ex.StackTrace, ex);
            }
        }

        private static async Task RunMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migrations");

            var migrations = InstanceProvider.GetInstances<Migration.Migration>(assembly, settings.Profile);
            foreach (var migration in migrations)
            {
                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                migration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);

            var orderedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp);
            foreach (var migration in orderedMigrations)
            {
                var versionInfo = operation.GetVersionInfo(
                    migration.Task.DatabaseName,
                    migration.Task.CollectionName);

                var task = migration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName);

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migration.Attribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                await operation.PerformTask(task, doc => task.Map(context.Log, doc));

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = migration.GetType().Name,
                    Description = task.Description,
                    Timestamp = migration.Attribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    migration.Task.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
        }

        private static async Task RunSqlMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Sql Migrations");

            var migrations = InstanceProvider.GetInstances<SqlMigration>(assembly, settings.Profile);
            foreach (var migration in migrations)
            {
                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                migration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);

            var orderedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp);
            foreach (var migration in orderedMigrations)
            {
                var versionInfo = operation.GetVersionInfo(
                    migration.Task.DatabaseName,
                    migration.Task.CollectionName);

                var task = migration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information,
                    "Running Migration: " + task.Description + " On Collection: " + task.CollectionName +
                    " On Database: " + task.DatabaseName);

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migration.Attribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        "Migration: " + task.Description + " On Collection: " + task.CollectionName +
                        " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var data = new Dictionary<string, IList<dynamic>>();

                foreach (var sqlStatement in task.SqlStatements)
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        $"Executing Sql Using Source {sqlStatement.DataSource} and Statement {sqlStatement.Statement}");
                    using (
                        var conn = new SqlConnection(GetConnection(sqlStatement.DataSource, settings.SqlConnections))
                    )
                    {
                        conn.Open();
                        data.Add(sqlStatement.StatementLookupKey ?? sqlStatement.DataSource, conn.Query<dynamic>(sqlStatement.Statement).ToList());
                    }
                }

                await operation.PerformTask(task, doc => task.Map(context.Log, doc, data));

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = migration.GetType().Name,
                    Description = task.Description,
                    Timestamp = migration.Attribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    migration.Task.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Sql Migrations");
        }

        private static async Task RunDocumentMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Document Migrations");

            var migrations = InstanceProvider.GetInstances<DocumentMigration>(assembly, settings.Profile);
            foreach (var migration in migrations)
            {
                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                migration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);
            var bulkOperation = new BulkDocumentOperations(settings.Connection, context);

            var orderedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp);
            foreach (var migration in orderedMigrations)
            {
                var versionInfo = operation.GetVersionInfo(
                    migration.Task.DatabaseName,
                    migration.Task.CollectionName);

                var task = migration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information,
                    "Running Migration: " + task.Description + " On Collection: " + task.CollectionName +
                    " On Database: " + task.DatabaseName);


                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migration.Attribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        "Migration: " + task.Description + " On Collection: " + task.CollectionName +
                        " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var data = new Dictionary<string, IList<JObject>>();

                foreach (var documentStatement in task.DocumentStatements)
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        $"Executing Document Query Using Source {documentStatement.DatabaseName} and Collection {documentStatement.CollectionName}");

                    var results = operation.GetDocuments(
                            documentStatement.DatabaseName,
                            documentStatement.CollectionName,
                            documentStatement.Filter);

                    data[documentStatement.AccessKey] = results;
                }

                await bulkOperation.PerformTask(task, doc => task.Map(context.Log, doc, data));

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = migration.GetType().Name,
                    Description = task.Description,
                    Timestamp = migration.Attribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    migration.Task.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Document Migrations");
        }

        private static async Task RunDataMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Document Migrations");

            var migrations = InstanceProvider.GetInstances<DataMigration>(assembly, settings.Profile);
            foreach (var migration in migrations)
            {
                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                migration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);

            var orderedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp);
            foreach (var migration in orderedMigrations)
            {
                var versionInfo = operation.GetVersionInfo(
                    migration.Task.DatabaseName,
                    migration.Task.CollectionName);

                var task = migration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information,
                    "Running Migration: " + task.Description + " On Collection: " + task.CollectionName +
                    " On Database: " + task.DatabaseName);


                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migration.Attribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        "Migration: " + task.Description + " On Collection: " + task.CollectionName +
                        " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var data = task.DataProvider(context.Log, settings);

                await operation.PerformTask(task, doc => task.Map(context.Log, doc, data));

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = migration.GetType().Name,
                    Description = task.Description,
                    Timestamp = migration.Attribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    migration.Task.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Document Migrations");
        }

        private static string GetConnection(string source, IEnumerable<SqlDatabaseConnectionSettings> settings)
        {
            var databaseConnectionDetail = settings.FirstOrDefault(cd => string.Equals(cd.DataSource, source, StringComparison.CurrentCultureIgnoreCase));

            return databaseConnectionDetail?.ConnectionString;
        }
    }
}
