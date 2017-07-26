using System;
using System.Diagnostics;
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
    public class Deletions
    {
        public static void Run(ICakeContext context, string assembly, DocumentDbMigrationSettings settings)
        {
            Debugger.Launch();
            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Deletions");

            var deletions = InstanceProvider.GetInstances<Deletion.DeleteDocuments>(assembly, settings.Profile);
            foreach (var migration in deletions)
            {
                var migrationAttribute = migration.GetType().GetCustomAttribute<MigrationAttribute>();

                migration.Attribute = migrationAttribute;
            }

            var operation = new DocumentOperations(settings.Connection, context);

            var attributedDeletions = deletions.Where(d => d.Attribute != null).OrderBy(d => d.Attribute.Timestamp).ToList();
            var runAlwaysDeletions = deletions.Where(d => d.Attribute == null).ToList();

            var orderedDeletions = attributedDeletions.Concat(runAlwaysDeletions).ToList();
            
            var groupedDeletions = orderedDeletions.GroupBy(m => m.Task.DatabaseName + "." + m.Task.CollectionName);

            foreach (var groupedDeletion in groupedDeletions)
            {
                var key = groupedDeletion.Key.Split('.');
                var versionInfo = operation.GetVersionInfo(
                        key[0],
                        key[1]);

                foreach (var deletion in groupedDeletion)
                {
                    var task = deletion.Task;

                    context.Log.Write(Verbosity.Normal, LogLevel.Information, "Running Deletion: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName);

                    if (deletion.Attribute != null &&
                        versionInfo.ProcessedMigrations.Any(pm =>
                            pm.Name == deletion.GetType().Name &&
                            pm.Timestamp == deletion.Attribute.Timestamp))
                    {
                        context.Log.Write(Verbosity.Normal, LogLevel.Information, "Deletion: " + task.Description + " On Collection: " + task.CollectionName + " On Database: " + task.DatabaseName + " Has Already Been Executed");
                        continue;
                    }

                    operation.DeleteDocuments(
                        task.DatabaseName,
                        task.CollectionName,
                        task.Filter,
                        task.PartitionKey,
                        task.PartitionKeyAccessor);

                    if (deletion.Attribute != null)
                    {
                        versionInfo.ProcessedMigrations.Add(new MigrationInfo
                        {
                            Name = deletion.GetType().Name,
                            Description = task.Description,
                            Timestamp = deletion.Attribute.Timestamp,
                            AppliedOn = DateTime.UtcNow
                        });
                    }
                }

                operation.UpsertVersionInfo(
                        key[0],
                        versionInfo);
            }

            context.Log.Write(Verbosity.Normal, LogLevel.Information, "Finished Running Deletions");
        }
    }
}
