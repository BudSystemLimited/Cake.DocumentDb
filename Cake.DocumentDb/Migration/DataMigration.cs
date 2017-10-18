using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration.Loqacious;

namespace Cake.DocumentDb.Migration
{
    public class DataMigration
    {
        internal MigrationAttribute Attribute { get; set; }

        internal DataMigrationTask Task { get; private set; }

        protected void Migrate(Action<DataMigrationConfigurationCreator> action)
        {
            var creator = new DataMigrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}