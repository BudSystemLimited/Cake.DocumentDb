using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Migration.Loqacious;

namespace Cake.DocumentDb.Migration
{
    public class Migration
    {
        internal MigrationAttribute Attribute { get; set; }

        internal MigrationTask Task { get; private set; }

        protected void Migrate(Action<MigrationConfigurationCreator> action)
        {
            var creator = new MigrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}