using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;
using Dapper;
using Newtonsoft.Json.Linq;

namespace Cake.DocumentDb.Operations
{
    public class Hydrations
    {
        public static async Task Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            try
            {
                RunSqlHydrations(context, assembly, settings);
                await RunDocumentHydrations(context, assembly, settings);
                RunDataHydrations(context, assembly, settings);
            }
            catch (Exception ex)
            {
                context.Log.Error(ex.Message);
                context.Log.Error(ex.StackTrace);
            }
        }

        private static void RunSqlHydrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Hydrations");

            var hydrations = InstanceProvider.GetInstances<SqlHydration>(assembly, settings.Profile);
            foreach (var hydration in hydrations)
            {
                var migrationAttribute = hydration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Hydration {hydration.GetType().Name} must have a migration attribute");

                hydration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);

            var orderedHydrations = hydrations.OrderBy(h => h.Attribute.Timestamp);
            foreach (var hydration in orderedHydrations)
            {
                var versionInfo = operation.GetVersionInfo(
                    hydration.Task.DatabaseName,
                    hydration.Task.CollectionName);
                var task = hydration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information,
                    "Running Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                    " On Database: " + task.DatabaseName);

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == hydration.GetType().Name &&
                    pm.Timestamp == hydration.Attribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        "Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                        " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var data = new Dictionary<string, IList<dynamic>>();

                if (task.AdditionalSqlStatements != null)
                {
                    foreach (var sqlStatement in task.AdditionalSqlStatements)
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Information,
                            $"Executing Sql Using Source {sqlStatement.DataSource} and Statement {sqlStatement.Statement}");
                        using (
                            var conn =
                                new SqlConnection(GetConnection(sqlStatement.DataSource, settings.SqlConnections))
                        )
                        {
                            conn.Open();
                            data.Add(sqlStatement.StatementLookupKey ?? sqlStatement.DataSource, conn.Query<dynamic>(sqlStatement.Statement).ToList());
                        }
                    }
                }

                using (var conn = new SqlConnection(GetConnection(task.SqlStatement.DataSource, settings.SqlConnections)))
                {
                    conn.Open();

                    var records = conn.Query<dynamic>(task.SqlStatement.Statement).ToList();

                    foreach (var record in records)
                    {
                        var document = task.DocumentCreator(context.Log, record, data);

                        operation.CreateDocument(
                            task.DatabaseName,
                            task.CollectionName,
                            document);
                    }
                }

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = hydration.GetType().Name,
                    Description = task.Description,
                    Timestamp = hydration.Attribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                        hydration.Task.DatabaseName,
                        versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Hydrations");
        }

        private static async Task RunDocumentHydrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Hydrations");

            var hydrations = InstanceProvider.GetInstances<DocumentHydration>(assembly, settings.Profile);
            foreach (var hydration in hydrations)
            {
                var migrationAttribute = hydration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Hydration {hydration.GetType().Name} must have a migration attribute");

                hydration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);

            var orderedHydrations = hydrations.OrderBy(h => h.Attribute.Timestamp);
            foreach (var hydration in orderedHydrations)
            {
                var versionInfo = operation.GetVersionInfo(
                    hydration.Task.DatabaseName,
                    hydration.Task.CollectionName);

                var task = hydration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information,
                    "Running Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                    " On Database: " + task.DatabaseName);

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == hydration.GetType().Name &&
                    pm.Timestamp == hydration.Attribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        "Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                        " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var data = new Dictionary<string, IList<JObject>>();

                if (task.AdditionalDocumentStatements != null)
                {
                    foreach (var sqlStatement in task.AdditionalDocumentStatements)
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Information,
                            $"Executing Sql Using Source {sqlStatement.DatabaseName} on Collection {sqlStatement.CollectionName}");

                        data.Add(sqlStatement.AccessKey, operation.GetDocuments(
                            sqlStatement.DatabaseName,
                            sqlStatement.CollectionName,
                            sqlStatement.Filter).ToList());
                    }
                }
                
                await operation.PerformHydrationTask(task, record =>
                {
                    var documents = new List<JObject>();

                    if (task.DocumentCreator != null)
                        documents.Add(task.DocumentCreator(context.Log, record, data));

                    if (task.DocumentsCreator != null)
                        documents.AddRange(task.DocumentsCreator(context.Log, record, data));

                    return documents;
                });

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = hydration.GetType().Name,
                    Description = task.Description,
                    Timestamp = hydration.Attribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                        hydration.Task.DatabaseName,
                        versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Hydrations");
        }

        private static void RunDataHydrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Hydrations");

            var hydrations = InstanceProvider.GetInstances<DataHydration>(assembly, settings.Profile);
            foreach (var hydration in hydrations)
            {
                var migrationAttribute = hydration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Hydration {hydration.GetType().Name} must have a migration attribute");

                hydration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);

            var orderedHydrations = hydrations.OrderBy(h => h.Attribute.Timestamp);

            foreach (var hydration in orderedHydrations)
            {
                var versionInfo = operation.GetVersionInfo(
                    hydration.Task.DatabaseName,
                    hydration.Task.CollectionName);

                var task = hydration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information,
                    "Running Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                    " On Database: " + task.DatabaseName);

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == hydration.GetType().Name &&
                    pm.Timestamp == hydration.Attribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        "Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                        " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var data = task.DataProvider(context.Log, settings);

                foreach (var record in data)
                {
                    var document = task.DocumentCreator(context.Log, record);

                    operation.CreateDocument(
                        task.DatabaseName,
                        task.CollectionName,
                        document);
                }

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = hydration.GetType().Name,
                    Description = task.Description,
                    Timestamp = hydration.Attribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    hydration.Task.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Hydrations");
        }

        private static string GetConnection(string source, IEnumerable<SqlDatabaseConnectionSettings> settings)
        {
            var databaseConnectionDetail = settings.FirstOrDefault(cd => string.Equals(cd.DataSource, source, StringComparison.CurrentCultureIgnoreCase));

            return databaseConnectionDetail?.ConnectionString;
        }
    }
}
