using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration;
using Cake.DocumentDb.Migration;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;
using Dapper;
using Newtonsoft.Json.Serialization;

namespace Cake.DocumentDb.Operations
{
    public class Hydrations
    {
        public static void Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            RunSqlHydrations(context, assembly, settings);
        }

        private static void RunSqlHydrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Hydrations");

            var hydrations = InstanceProvider.GetInstances<SqlHydration>(assembly, settings.Profile);

            var operation = new DocumentOperations(settings.Connection, context);

            var groupedHydrations = hydrations.GroupBy(m => m.Task.DatabaseName + "." + m.Task.CollectionName);

            foreach (var groupedHydration in groupedHydrations)
            {
                var key = groupedHydration.Key.Split('.');
                var versionInfo = operation.GetVersionInfo(
                    key[0],
                    key[1]);

                foreach (var hydration in hydrations)
                {
                    var task = hydration.Task;

                    context.Log.Write(Verbosity.Normal, LogLevel.Information,
                        "Running Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                        " On Database: " + task.DatabaseName);

                    var migrationAttribute = hydration.GetType().GetCustomAttribute<MigrationAttribute>();

                    if (migrationAttribute == null)
                        throw new InvalidOperationException(
                            $"Hydration {hydration.GetType().Name} must have a migration attribute");

                    if (versionInfo.ProcessedMigrations.Any(pm =>
                        pm.Name == hydration.GetType().Name &&
                        pm.Timestamp == migrationAttribute.Timestamp))
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Information,
                            "Hydration: " + task.Description + " On Collection: " + task.CollectionName +
                            " On Database: " + task.DatabaseName + " Has Already Been Executed");
                        continue;
                    }

                    using (var conn = new SqlConnection(GetConnection(task.SqlStatement.DataSource, settings.SqlConnections)))
                    {
                        conn.Open();

                        var records = conn.Query<dynamic>(task.SqlStatement.Statement).ToList();

                        foreach (var record in records)
                        {
                            var document = task.DocumentCreator(context.Log, record);

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
                        Timestamp = migrationAttribute.Timestamp,
                        AppliedOn = DateTime.UtcNow
                    });
                }

                operation.UpsertVersionInfo(
                        key[0],
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
