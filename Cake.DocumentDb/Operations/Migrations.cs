using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration;
using Cake.DocumentDb.Providers;
using Cake.DocumentDb.Requests;
using Dapper;

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

            var migrations = InstanceProvider.GetInstances<Migration.Migration>(assembly, settings.Profile);

            var operation = new DocumentOperations(settings.Connection, context);

            foreach (var migration in migrations)
            {
                var task = migration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName);

                var versionInfo = operation.GetVersionInfo(
                    task.DatabaseName,
                    task.CollectionName);

                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migrationAttribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var documents = operation.GetDocuments(
                    task.DatabaseName,
                    task.CollectionName);

                foreach (var document in documents)
                {
                    task.Map(context.Log, document);

                    operation.UpsertDocument(
                        task.DatabaseName,
                        task.CollectionName,
                        document);
                }

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = migration.GetType().Name,
                    Description = task.Description,
                    Timestamp = migrationAttribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    task.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
        }

        private static void RunSqlMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migrations");

            var migrations = InstanceProvider.GetInstances<SqlMigration>(assembly, settings.Profile);

            var operation = new DocumentOperations(settings.Connection, context);

            foreach (var migration in migrations)
            {
                var task = migration.Task;

                context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName);

                var versionInfo = operation.GetVersionInfo(
                    task.DatabaseName,
                    task.CollectionName);

                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                if (migrationAttribute == null)
                    throw new InvalidOperationException($"Migration {migration.GetType().Name} must have a migration attribute");

                if (versionInfo.ProcessedMigrations.Any(pm =>
                    pm.Name == migration.GetType().Name &&
                    pm.Timestamp == migrationAttribute.Timestamp))
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName + " Has Already Been Executed");
                    continue;
                }

                var data = new Dictionary<string, IList<dynamic>>();

                foreach (var sqlStatement in task.SqlStatements)
                {
                    context.Log.Write(Verbosity.Normal, LogLevel.Information, $"Executing Sql Using Source {sqlStatement.DataSource} and Statement {sqlStatement.Statement}");
                    using (var conn = new SqlConnection(GetConnection(sqlStatement.DataSource, settings.SqlConnection)))
                    {
                        conn.Open();
                        data.Add(sqlStatement.DataSource, conn.Query<dynamic>(sqlStatement.Statement).ToList());
                    }
                }

                var documents = operation.GetDocuments(
                    task.DatabaseName,
                    task.CollectionName);

                foreach (var document in documents)
                {
                    task.Map(context.Log, document, data);

                    operation.UpsertDocument(
                        task.DatabaseName,
                        task.CollectionName,
                        document);
                }

                versionInfo.ProcessedMigrations.Add(new MigrationInfo
                {
                    Name = migration.GetType().Name,
                    Description = task.Description,
                    Timestamp = migrationAttribute.Timestamp,
                    AppliedOn = DateTime.UtcNow
                });

                operation.UpsertVersionInfo(
                    task.DatabaseName,
                    versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
        }

        private static string GetConnection(string source, IEnumerable<SqlDatabaseConnectionSettings> settings)
        {
            var databaseConnectionDetail = settings.FirstOrDefault(cd => string.Equals(cd.DataSource, source, StringComparison.CurrentCultureIgnoreCase));

            return databaseConnectionDetail?.ConnectionString;
        }
    }
}
