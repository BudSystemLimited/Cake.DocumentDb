using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration.Loqacious;

namespace Cake.DocumentDb.Migration
{
    public class DocumentMigration
    {
        internal MigrationAttribute Attribute { get; set; }

        internal DocumentMigrationTask Task { get; private set; }

        protected void Migrate(Action<DocumentMigrationConfigurationCreator> action)
        {
            var creator = new DocumentMigrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}