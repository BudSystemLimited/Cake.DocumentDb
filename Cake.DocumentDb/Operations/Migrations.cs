﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        public static void Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            RunMigrations(context, assembly, settings);
            RunSqlMigrations(context, assembly, settings);
            RunDocumentMigrations(context, assembly, settings);
            RunDataMigrations(context, assembly, settings);
        }

        private static void RunMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
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

            var groupedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp)
                                              .GroupBy(m => m.Task.DatabaseName + "." + m.Task.CollectionName);

            foreach (var groupedMigration in groupedMigrations)
            {
                var key = groupedMigration.Key.Split('.');
                var versionInfo = operation.GetVersionInfo(
                        key[0],
                        key[1]);


                foreach (var migration in groupedMigration)
                {
                    var task = migration.Task;

                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName);

                    if (versionInfo.ProcessedMigrations.Any(pm =>
                        pm.Name == migration.GetType().Name &&
                        pm.Timestamp == migration.Attribute.Timestamp))
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Information, "Migration: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName + " Has Already Been Executed");
                        continue;
                    }

                    operation.PerformTask(task, doc => task.Map(context.Log, doc));

                    versionInfo.ProcessedMigrations.Add(new MigrationInfo
                    {
                        Name = migration.GetType().Name,
                        Description = task.Description,
                        Timestamp = migration.Attribute.Timestamp,
                        AppliedOn = DateTime.UtcNow
                    });
                }

                operation.UpsertVersionInfo(
                        key[0],
                        versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Seeds");
        }

        private static void RunSqlMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
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

            var groupedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp)
                                              .GroupBy(m => m.Task.DatabaseName + "." + m.Task.CollectionName);

            foreach (var groupedMigration in groupedMigrations)
            {
                var key = groupedMigration.Key.Split('.');
                var versionInfo = operation.GetVersionInfo(
                    key[0],
                    key[1]);

                foreach (var migration in groupedMigration)
                {
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
                    
                    operation.PerformTask(task, doc => task.Map(context.Log, doc, data));

                    versionInfo.ProcessedMigrations.Add(new MigrationInfo
                    {
                        Name = migration.GetType().Name,
                        Description = task.Description,
                        Timestamp = migration.Attribute.Timestamp,
                        AppliedOn = DateTime.UtcNow
                    });
                }

                operation.UpsertVersionInfo(
                        key[0],
                        versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Sql Migrations");
        }

        private static void RunDocumentMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
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

            var groupedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp)
                                              .GroupBy(m => m.Task.DatabaseName + "." + m.Task.CollectionName);

            foreach (var groupedMigration in groupedMigrations)
            {
                var key = groupedMigration.Key.Split('.');
                var versionInfo = operation.GetVersionInfo(
                    key[0],
                    key[1]);

                foreach (var migration in groupedMigration)
                {
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

                    operation.PerformTask(task, doc => task.Map(context.Log, doc, data));

                    versionInfo.ProcessedMigrations.Add(new MigrationInfo
                    {
                        Name = migration.GetType().Name,
                        Description = task.Description,
                        Timestamp = migration.Attribute.Timestamp,
                        AppliedOn = DateTime.UtcNow
                    });
                }

                operation.UpsertVersionInfo(
                        key[0],
                        versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Document Migrations");
        }

        private static void RunDataMigrations(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
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

            var groupedMigrations = migrations.OrderBy(m => m.Attribute.Timestamp)
                                              .GroupBy(m => m.Task.DatabaseName + "." + m.Task.CollectionName);

            foreach (var groupedMigration in groupedMigrations)
            {
                var key = groupedMigration.Key.Split('.');
                var versionInfo = operation.GetVersionInfo(
                    key[0],
                    key[1]);

                foreach (var migration in groupedMigration)
                {
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

                    operation.PerformTask(task, doc => task.Map(context.Log, doc, data));

                    versionInfo.ProcessedMigrations.Add(new MigrationInfo
                    {
                        Name = migration.GetType().Name,
                        Description = task.Description,
                        Timestamp = migration.Attribute.Timestamp,
                        AppliedOn = DateTime.UtcNow
                    });
                }

                operation.UpsertVersionInfo(
                        key[0],
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
