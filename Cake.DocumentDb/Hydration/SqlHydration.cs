using System;
using Cake.DocumentDb.Attributes;
using Cake.DocumentDb.Hydration.Loqacious;

namespace Cake.DocumentDb.Hydration
{
    public class SqlHydration
    {
        internal MigrationAttribute Attribute { get; set; }

        internal SqlHydrationTask Task { get; private set; }

        protected void Migrate(Action<SqlHydrationConfigurationCreator> action)
        {
            var creator = new SqlHydrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}