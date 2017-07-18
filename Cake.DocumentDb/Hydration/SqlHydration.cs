using System;
using Cake.DocumentDb.Hydration.Loqacious;

namespace Cake.DocumentDb.Hydration
{
    public class SqlHydration
    {
        internal SqlHydrationTask Task { get; private set; }

        protected void Migrate(Action<SqlHydrationConfigurationCreator> action)
        {
            var creator = new SqlHydrationConfigurationCreator();

            action(creator);

            Task = creator.MigrationTask;
        }
    }
}