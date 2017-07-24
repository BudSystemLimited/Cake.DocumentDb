using System;
using Cake.DocumentDb.Migration.Loqacious;

namespace Cake.DocumentDb.Migration
{
    public class DocumentMigration
    {
        internal DocumentMigrationTask Task { get; private set; }

        protected void Migrate(Action<DocumentMigrationConfigurationCreator> action)
        {
            var creator = new DocumentMigrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}