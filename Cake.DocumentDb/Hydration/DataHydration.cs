using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration.Loqacious;

namespace Cake.DocumentDb.Hydration
{
    public class DataHydration
    {
        internal MigrationAttribute Attribute { get; set; }

        internal DataHydrationTask Task { get; private set; }

        protected void Migrate(Action<DataHydrationConfigurationCreator> action)
        {
            var creator = new DataHydrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}
