using System;
using Cake.DocumentDb.Migration.Loqacious;

namespace Cake.DocumentDb.Migration
{
    public class SqlMigration
    {
        internal SqlMigrationTask Task { get; private set; }

        protected void Migrate(Action<SqlMigrationConfigurationCreator> action)
        {
            var creator = new SqlMigrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}